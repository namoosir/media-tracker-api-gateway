using System.Security.Claims;

namespace MediaTrackerApiGateway.Services.SessionTokenService
{
    public interface ISessionTokenService
    {
        bool ValidateToken(string token, out ClaimsPrincipal claimsPrincipal);

        int? GetUserIdFromClaimsPrincipal(ClaimsPrincipal claim);
    }
}
