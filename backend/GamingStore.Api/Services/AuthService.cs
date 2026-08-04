using System.Security.Claims;

namespace GamingStore.Api.Services;

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

    Task LogoutAsync();
}

public sealed class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager) : IAuthService
{
    private const string InvalidLoginMessage = "Invalid email or password.";

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

        return MapToResponse(user);
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

        return MapToResponse(user);
    }

    public async Task<AuthenticatedUserResponse> GetCurrentUserAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.GetUserAsync(principal)
            ?? throw new AuthenticationFailedException("The authenticated account is unavailable.");

        return MapToResponse(user);
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

    private static AuthenticatedUserResponse MapToResponse(User user)
    {
        return new AuthenticatedUserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email ?? string.Empty);
    }
}
