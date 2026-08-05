using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GamingStore.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthService authService,
    IAntiforgery antiforgery,
    IAuthenticationSchemeProvider authenticationSchemeProvider) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("antiforgery-token")]
    [EnableRateLimiting("authentication")]
    public ActionResult<AntiforgeryTokenResponse> GetAntiforgeryToken()
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        Response.Headers.CacheControl = "no-store";

        return Ok(new AntiforgeryTokenResponse(tokens.RequestToken!));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [EnableRateLimiting("authentication")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<AuthenticatedUserResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var user = await authService.RegisterAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, user);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [EnableRateLimiting("authentication")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<AuthenticatedUserResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await authService.LoginAsync(request, cancellationToken);

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpGet("google")]
    [EnableRateLimiting("authentication")]
    public async Task<IActionResult> GoogleLogin()
    {
        if (!await IsGoogleAuthenticationAvailableAsync())
        {
            return LocalRedirect("/login?authError=google-not-configured");
        }

        var properties = authService.ConfigureExternalAuthenticationProperties(
            GoogleAuthentication.Scheme,
            "/api/auth/google/complete");

        return Challenge(properties, GoogleAuthentication.Scheme);
    }

    [AllowAnonymous]
    [HttpGet("google/complete")]
    public async Task<IActionResult> CompleteGoogleLogin(CancellationToken cancellationToken)
    {
        var outcome = await authService.CompleteExternalLoginAsync(cancellationToken);

        return LocalRedirect(outcome switch
        {
            ExternalAuthenticationOutcome.Succeeded => "/?authStatus=google-signed-in",
            ExternalAuthenticationOutcome.ExistingLocalAccount =>
                "/login?authError=google-account-exists",
            ExternalAuthenticationOutcome.LockedOut => "/login?authError=locked-out",
            _ => "/login?authError=google-failed"
        });
    }

    [Authorize]
    [HttpGet("google/link")]
    [EnableRateLimiting("authentication")]
    public async Task<IActionResult> LinkGoogle()
    {
        if (!await IsGoogleAuthenticationAvailableAsync())
        {
            return LocalRedirect("/?authError=google-not-configured");
        }

        var properties = authService.ConfigureExternalAuthenticationProperties(
            GoogleAuthentication.Scheme,
            "/api/auth/google/link-complete",
            User);

        return Challenge(properties, GoogleAuthentication.Scheme);
    }

    [Authorize]
    [HttpGet("google/link-complete")]
    public async Task<IActionResult> CompleteGoogleLink(CancellationToken cancellationToken)
    {
        var outcome = await authService.LinkExternalLoginAsync(User, cancellationToken);

        return LocalRedirect(outcome switch
        {
            ExternalAuthenticationOutcome.Succeeded or ExternalAuthenticationOutcome.AlreadyLinked =>
                "/?authStatus=google-linked",
            ExternalAuthenticationOutcome.ProviderAccountInUse =>
                "/?authError=google-in-use",
            _ => "/?authError=google-link-failed"
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<AuthenticatedUserResponse>> Me(CancellationToken cancellationToken)
    {
        var user = await authService.GetCurrentUserAsync(User, cancellationToken);

        return Ok(user);
    }

    [Authorize]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync();

        return NoContent();
    }

    private async Task<bool> IsGoogleAuthenticationAvailableAsync()
    {
        return await authenticationSchemeProvider.GetSchemeAsync(GoogleAuthentication.Scheme)
            is not null;
    }
}
