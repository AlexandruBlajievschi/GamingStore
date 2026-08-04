using System.Buffers.Binary;
using GamingStore.Api.Data;
using GamingStore.Api.DTOs;
using GamingStore.Api.Models;
using GamingStore.Api.Models.Entities;
using GamingStore.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GamingStore.Api.IntegrationTests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_PersistsIdentityPasswordHashAndSignsIn()
    {
        await using var host = await AuthTestHost.CreateAsync();
        var request = new RegisterRequest(
            " Alex ",
            " Player ",
            " ALEX.PLAYER@EXAMPLE.COM ",
            "correct horse battery staple");

        var response = await host.AuthService.RegisterAsync(request, CancellationToken.None);

        var storedUser = await host.UserManager.FindByIdAsync(response.Id.ToString());
        Assert.NotNull(storedUser);
        Assert.Equal("Alex", storedUser.FirstName);
        Assert.Equal("alex.player@example.com", storedUser.Email);
        Assert.NotNull(storedUser.PasswordHash);
        Assert.DoesNotContain(request.Password, storedUser.PasswordHash, StringComparison.Ordinal);
        Assert.True(await host.UserManager.CheckPasswordAsync(storedUser, request.Password));
        Assert.Equal(220_000, ReadIterationCount(storedUser.PasswordHash));
        Assert.Contains(
            host.HttpContext.Response.Headers.SetCookie,
            value => value?.Contains("Identity.Application", StringComparison.Ordinal) == true);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsGenericError_WhenEmailAlreadyExists()
    {
        await using var host = await AuthTestHost.CreateAsync();
        var firstRequest = new RegisterRequest(
            "Alex",
            "Player",
            "alex.player@example.com",
            "correct horse battery staple");
        var duplicateRequest = firstRequest with { FirstName = "Other" };
        await host.AuthService.RegisterAsync(firstRequest, CancellationToken.None);

        var exception = await Assert.ThrowsAsync<DomainValidationException>(
            () => host.AuthService.RegisterAsync(duplicateRequest, CancellationToken.None));

        Assert.Equal(
            "Registration could not be completed with the supplied account details.",
            exception.Message);
    }

    [Fact]
    public async Task LoginAsync_ReturnsSameGenericError_ForUnknownEmailAndWrongPassword()
    {
        await using var host = await AuthTestHost.CreateAsync();
        var password = "correct horse battery staple";
        await host.AuthService.RegisterAsync(
            new RegisterRequest("Alex", "Player", "alex.player@example.com", password),
            CancellationToken.None);

        var unknownEmail = await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => host.AuthService.LoginAsync(
                new LoginRequest("missing@example.com", password),
                CancellationToken.None));
        var wrongPassword = await Assert.ThrowsAsync<AuthenticationFailedException>(
            () => host.AuthService.LoginAsync(
                new LoginRequest("alex.player@example.com", "this password is incorrect"),
                CancellationToken.None));

        Assert.Equal("Invalid email or password.", unknownEmail.Message);
        Assert.Equal(unknownEmail.Message, wrongPassword.Message);
    }

    private static int ReadIterationCount(string passwordHash)
    {
        var bytes = Convert.FromBase64String(passwordHash);

        Assert.Equal(0x01, bytes[0]);

        return checked((int)BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(5, 4)));
    }

    private sealed class AuthTestHost : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ServiceProvider _serviceProvider;
        private readonly IServiceScope _scope;
        private readonly IAuthService _authService;
        private readonly SignInManager<User> _signInManager;

        private AuthTestHost(
            SqliteConnection connection,
            ServiceProvider serviceProvider,
            IServiceScope scope,
            DefaultHttpContext httpContext)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
            _scope = scope;
            HttpContext = httpContext;
            _authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            _signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<User>>();
            UserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        }

        public IAuthService AuthService
        {
            get
            {
                _signInManager.Context = HttpContext;

                return _authService;
            }
        }

        public UserManager<User> UserManager { get; }

        public DefaultHttpContext HttpContext { get; }

        public static async Task<AuthTestHost> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpContextAccessor();
            services.AddDataProtection().UseEphemeralDataProtectionProvider();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));
            services
                .AddIdentity<User, IdentityRole<Guid>>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 15;
                    options.Password.RequiredUniqueChars = 1;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<PasswordHasherOptions>(options =>
                options.IterationCount = 220_000);
            services.AddScoped<IAuthService, AuthService>();

            var serviceProvider = services.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = scope.ServiceProvider
            };
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("gamingstore.test");
            scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = httpContext;

            return new AuthTestHost(connection, serviceProvider, scope, httpContext);
        }

        public async ValueTask DisposeAsync()
        {
            _scope.Dispose();
            await _serviceProvider.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
