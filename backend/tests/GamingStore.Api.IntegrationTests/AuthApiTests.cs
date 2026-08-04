using System.Net;
using System.Net.Http.Json;
using GamingStore.Api.Data;
using GamingStore.Api.DTOs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GamingStore.Api.IntegrationTests;

public sealed class AuthApiTests
{
    [Fact]
    public async Task Register_IssuesCookieThatAuthenticatesCurrentUserRequest()
    {
        await using var factory = new AuthApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
        var antiforgery = await client.GetFromJsonAsync<AntiforgeryTokenResponse>(
            "/api/auth/antiforgery-token");
        using var registerRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/register")
        {
            Content = JsonContent.Create(new RegisterRequest(
                "HTTP",
                "Tester",
                "http.tester@example.com",
                "correct horse battery staple"))
        };
        registerRequest.Headers.Add("X-CSRF-TOKEN", antiforgery!.Token);

        using var registerResponse = await client.SendAsync(registerRequest);

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        var registeredUser = await registerResponse.Content.ReadFromJsonAsync<AuthenticatedUserResponse>();
        Assert.NotNull(registeredUser);
        Assert.Equal("http.tester@example.com", registeredUser.Email);

        var currentUser = await client.GetFromJsonAsync<AuthenticatedUserResponse>("/api/auth/me");
        Assert.Equal(registeredUser, currentUser);
    }

    private sealed class AuthApiFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection? _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureLogging(logging => logging.ClearProviders());
            builder.ConfigureAppConfiguration(configuration =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=unused"
                });
            });
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();

                _connection = new SqliteConnection("Data Source=:memory:");
                _connection.Open();

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(_connection));
                services.AddDataProtection().UseEphemeralDataProtectionProvider();
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();

            return host;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _connection?.Dispose();
            }
        }
    }
}
