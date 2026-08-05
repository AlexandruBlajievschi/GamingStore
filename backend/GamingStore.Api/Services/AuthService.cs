using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace GamingStore.Api.Services;

public enum ExternalAuthenticationOutcome
{
    Succeeded,
    ExistingLocalAccount,
    LockedOut,
    AlreadyLinked,
    ProviderAccountInUse,
    Failed
}

public interface IAuthService
{
    Task<AuthenticatedUserResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken);

    Task<AuthenticatedUserResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    Task<AuthenticatedUserResponse> GetCurrentUserAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken);

    AuthenticationProperties ConfigureExternalAuthenticationProperties(
        string provider,
        string redirectUrl,
        ClaimsPrincipal? linkingUser = null);

    Task<ExternalAuthenticationOutcome> CompleteExternalLoginAsync(
        CancellationToken cancellationToken);

    Task<ExternalAuthenticationOutcome> LinkExternalLoginAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken);

    Task LogoutAsync();
}

public sealed class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ILogger<AuthService> logger) : IAuthService
{
    private const string InvalidLoginMessage = "Invalid email or password.";
    private const int NameMaxLength = 100;

    public async Task<AuthenticatedUserResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = User.Create(request.FirstName, request.LastName, request.Email);
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new DomainValidationException(CreateRegistrationError(result));
        }

        await signInManager.SignInAsync(user, isPersistent: false);

        return await MapToResponseAsync(user);
    }

    public async Task<AuthenticatedUserResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var email = request.Email.Trim().ToLowerInvariant();
        var result = await signInManager.PasswordSignInAsync(
            email,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            throw new AuthenticationFailedException(InvalidLoginMessage);
        }

        var user = await userManager.FindByEmailAsync(email)
            ?? throw new AuthenticationFailedException(InvalidLoginMessage);

        return await MapToResponseAsync(user);
    }

    public async Task<AuthenticatedUserResponse> GetCurrentUserAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.GetUserAsync(principal)
            ?? throw new AuthenticationFailedException("The authenticated account is unavailable.");

        return await MapToResponseAsync(user);
    }

    public AuthenticationProperties ConfigureExternalAuthenticationProperties(
        string provider,
        string redirectUrl,
        ClaimsPrincipal? linkingUser = null)
    {
        var userId = linkingUser is null
            ? null
            : userManager.GetUserId(linkingUser)
                ?? throw new AuthenticationFailedException(
                    "The authenticated account is unavailable.");

        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            provider,
            redirectUrl,
            userId);

        if (linkingUser is not null)
        {
            properties.Items[GoogleAuthentication.FlowProperty] = GoogleAuthentication.LinkFlow;
        }

        return properties;
    }

    public async Task<ExternalAuthenticationOutcome> CompleteExternalLoginAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var info = await signInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            return ExternalAuthenticationOutcome.Failed;
        }

        try
        {
            return await CompleteExternalLoginAsync(info, cancellationToken);
        }
        finally
        {
            await signInManager.Context.SignOutAsync(IdentityConstants.ExternalScheme);
        }
    }

    public async Task<ExternalAuthenticationOutcome> LinkExternalLoginAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.GetUserAsync(principal)
            ?? throw new AuthenticationFailedException(
                "The authenticated account is unavailable.");
        var info = await signInManager.GetExternalLoginInfoAsync(user.Id.ToString());

        if (info is null)
        {
            return ExternalAuthenticationOutcome.Failed;
        }

        try
        {
            return await LinkExternalLoginAsync(user, info, cancellationToken);
        }
        finally
        {
            await signInManager.Context.SignOutAsync(IdentityConstants.ExternalScheme);
        }
    }

    public async Task<ExternalAuthenticationOutcome> CompleteExternalLoginAsync(
        ExternalLoginInfo info,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (info.LoginProvider != GoogleAuthentication.Scheme)
        {
            return ExternalAuthenticationOutcome.Failed;
        }

        var signInResult = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: false);

        if (signInResult.Succeeded)
        {
            return ExternalAuthenticationOutcome.Succeeded;
        }

        if (signInResult.IsLockedOut)
        {
            return ExternalAuthenticationOutcome.LockedOut;
        }

        if (await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey) is not null)
        {
            return ExternalAuthenticationOutcome.Failed;
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email) || !HasVerifiedGoogleEmail(info.Principal))
        {
            logger.LogWarning("Google authentication did not provide a verified email address.");
            return ExternalAuthenticationOutcome.Failed;
        }

        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return ExternalAuthenticationOutcome.ExistingLocalAccount;
        }

        var (firstName, lastName) = GetNames(info.Principal);
        var user = User.Create(firstName, lastName, email);
        user.EmailConfirmed = true;
        var createResult = await userManager.CreateAsync(user);

        if (!createResult.Succeeded)
        {
            logger.LogWarning(
                "Could not create a Google-backed user. Identity errors: {ErrorCodes}",
                string.Join(", ", createResult.Errors.Select(error => error.Code)));

            return HasDuplicateAccountError(createResult)
                ? ExternalAuthenticationOutcome.ExistingLocalAccount
                : ExternalAuthenticationOutcome.Failed;
        }

        var addLoginResult = await userManager.AddLoginAsync(user, info);

        if (!addLoginResult.Succeeded)
        {
            logger.LogWarning(
                "Could not attach a Google login to a new user. Identity errors: {ErrorCodes}",
                string.Join(", ", addLoginResult.Errors.Select(error => error.Code)));
            await userManager.DeleteAsync(user);

            return ExternalAuthenticationOutcome.Failed;
        }

        await signInManager.SignInAsync(
            user,
            isPersistent: false,
            authenticationMethod: info.LoginProvider);

        return ExternalAuthenticationOutcome.Succeeded;
    }

    public async Task<ExternalAuthenticationOutcome> LinkExternalLoginAsync(
        User user,
        ExternalLoginInfo info,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (info.LoginProvider != GoogleAuthentication.Scheme)
        {
            return ExternalAuthenticationOutcome.Failed;
        }

        var loginOwner = await userManager.FindByLoginAsync(
            info.LoginProvider,
            info.ProviderKey);

        if (loginOwner?.Id == user.Id)
        {
            return ExternalAuthenticationOutcome.AlreadyLinked;
        }

        if (loginOwner is not null)
        {
            return ExternalAuthenticationOutcome.ProviderAccountInUse;
        }

        var result = await userManager.AddLoginAsync(user, info);

        if (!result.Succeeded)
        {
            logger.LogWarning(
                "Could not attach Google to an existing user. Identity errors: {ErrorCodes}",
                string.Join(", ", result.Errors.Select(error => error.Code)));
            return ExternalAuthenticationOutcome.Failed;
        }

        return ExternalAuthenticationOutcome.Succeeded;
    }

    public Task LogoutAsync()
    {
        return signInManager.SignOutAsync();
    }

    private static string CreateRegistrationError(IdentityResult result)
    {
        if (result.Errors.Any(error =>
                error.Code is "DuplicateEmail" or "DuplicateUserName"))
        {
            return "Registration could not be completed with the supplied account details.";
        }

        return string.Join(" ", result.Errors.Select(error => error.Description));
    }

    private async Task<AuthenticatedUserResponse> MapToResponseAsync(User user)
    {
        var logins = await userManager.GetLoginsAsync(user);

        return new AuthenticatedUserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email ?? string.Empty,
            logins.Any(login => login.LoginProvider == GoogleAuthentication.Scheme));
    }

    private static bool HasVerifiedGoogleEmail(ClaimsPrincipal principal)
    {
        return bool.TryParse(
                principal.FindFirstValue(GoogleAuthentication.EmailVerifiedClaim),
                out var isVerified)
            && isVerified;
    }

    private static bool HasDuplicateAccountError(IdentityResult result)
    {
        return result.Errors.Any(error =>
            error.Code is "DuplicateEmail" or "DuplicateUserName");
    }

    private static (string FirstName, string LastName) GetNames(ClaimsPrincipal principal)
    {
        var firstName = principal.FindFirstValue(ClaimTypes.GivenName)?.Trim();
        var lastName = principal.FindFirstValue(ClaimTypes.Surname)?.Trim();
        var fullName = principal.FindFirstValue(ClaimTypes.Name)?.Trim();

        if ((string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            && !string.IsNullOrWhiteSpace(fullName))
        {
            var nameParts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            firstName = string.IsNullOrWhiteSpace(firstName) ? nameParts[0] : firstName;
            lastName = string.IsNullOrWhiteSpace(lastName) && nameParts.Length > 1
                ? nameParts[1]
                : lastName;
        }

        return (
            LimitName(string.IsNullOrWhiteSpace(firstName) ? "Google" : firstName),
            LimitName(string.IsNullOrWhiteSpace(lastName) ? "User" : lastName));
    }

    private static string LimitName(string name)
    {
        name = name.Trim();

        return name.Length <= NameMaxLength ? name : name[..NameMaxLength];
    }
}
