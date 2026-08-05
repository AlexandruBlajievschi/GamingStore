using System.Security.Claims;
using System.Text.Json;
using GamingStore.Api.Models;
using Microsoft.AspNetCore.Authentication.Google;

namespace GamingStore.Api.UnitTests;

public sealed class GoogleAuthenticationTests
{
    [Fact]
    public void MapClaims_MapsV3EmailVerifiedField()
    {
        var options = new GoogleOptions();
        GoogleAuthentication.MapClaims(options);
        var identity = new ClaimsIdentity();
        using var userInfo = JsonDocument.Parse("""{"email_verified":true}""");

        foreach (var claimAction in options.ClaimActions)
        {
            claimAction.Run(userInfo.RootElement, identity, GoogleAuthentication.Scheme);
        }

        var claim = Assert.Single(
            identity.FindAll(GoogleAuthentication.EmailVerifiedClaim));
        Assert.True(bool.Parse(claim.Value));
    }
}
