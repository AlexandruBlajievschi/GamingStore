using System.Security.Claims;
using GamingStore.Api.Controllers;
using GamingStore.Api.DTOs;
using GamingStore.Api.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace GamingStore.Api.UnitTests;

public sealed class AuthControllerTests
{
    private static readonly Guid UserId = Guid.Parse("3b71bd55-f63b-471a-8400-5be6ad615b84");
    private static readonly AuthenticatedUserResponse Response = new(
        UserId,
        "Alex",
        "Player",
        "alex.player@example.com");

    [Fact]
    public async Task Register_ReturnsCreatedUser()
    {
        var controller = CreateController(out _);
        var request = new RegisterRequest(
            "Alex",
            "Player",
            "alex.player@example.com",
            "correct horse battery staple");

        var result = await controller.Register(request, CancellationToken.None);

        var created = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.Equal(Response, created.Value);
    }

    [Fact]
    public async Task Login_ReturnsAuthenticatedUser()
    {
        var controller = CreateController(out _);
        var request = new LoginRequest(
            "alex.player@example.com",
            "correct horse battery staple");

        var result = await controller.Login(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(Response, ok.Value);
    }

    [Fact]
    public async Task Me_ReturnsCurrentUser()
    {
        var controller = CreateController(out _);

        var result = await controller.Me(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(Response, ok.Value);
    }

    [Fact]
    public async Task Logout_ReturnsNoContent()
    {
        var controller = CreateController(out var service);

        var result = await controller.Logout();

        Assert.IsType<NoContentResult>(result);
        Assert.True(service.LoggedOut);
    }

    private static AuthController CreateController(out FakeAuthService service)
    {
        service = new FakeAuthService();
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddAntiforgery();
        serviceCollection.AddAuthentication();
        var services = serviceCollection.BuildServiceProvider();
        var controller = new AuthController(
            service,
            services.GetRequiredService<IAntiforgery>(),
            services.GetRequiredService<IAuthenticationSchemeProvider>());
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = services,
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        return controller;
    }

    private sealed class FakeAuthService : IAuthService
    {
        public bool LoggedOut { get; private set; }

        public Task<AuthenticatedUserResponse> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Response);
        }

        public Task<AuthenticatedUserResponse> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Response);
        }

        public Task<AuthenticatedUserResponse> GetCurrentUserAsync(
            ClaimsPrincipal principal,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Response);
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(
            string provider,
            string redirectUrl,
            ClaimsPrincipal? linkingUser = null)
        {
            return new AuthenticationProperties { RedirectUri = redirectUrl };
        }

        public Task<ExternalAuthenticationOutcome> CompleteExternalLoginAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult(ExternalAuthenticationOutcome.Succeeded);
        }

        public Task<ExternalAuthenticationOutcome> LinkExternalLoginAsync(
            ClaimsPrincipal principal,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(ExternalAuthenticationOutcome.Succeeded);
        }

        public Task LogoutAsync()
        {
            LoggedOut = true;

            return Task.CompletedTask;
        }
    }
}
