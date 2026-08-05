using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace GamingStore.Api.Models;

public static class GoogleAuthentication
{
    public const string Scheme = "Google";
    public const string EmailVerifiedClaim = "gamingstore:google:email_verified";
    public const string FlowProperty = "gamingstore:google-flow";
    public const string LinkFlow = "link";

    public static void MapClaims(GoogleOptions options)
    {
        options.ClaimActions.MapJsonKey(
            EmailVerifiedClaim,
            "email_verified",
            ClaimValueTypes.Boolean);
    }
}
