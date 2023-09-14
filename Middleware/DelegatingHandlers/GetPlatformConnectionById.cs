using System.Security.Claims;
using MediaTrackerApiGateway.Services.SessionTokenService;

namespace MediaTrackerApiGateway.Middleware.DelegatingHandlers;

public class GetPlatformConnectionById : DelegatingHandler
{
    private readonly ISessionTokenService _sessionTokenService;

    public GetPlatformConnectionById(ISessionTokenService sessionTokenService)
        : base()
    {
        _sessionTokenService = sessionTokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        string token = request.Headers
            .GetValues("Authorization")
            .FirstOrDefault()!
            .Replace("Bearer ", "");

        _sessionTokenService.ValidateToken(token, out ClaimsPrincipal claimsPrincipal);

        int? userId = _sessionTokenService.GetUserIdFromClaimsPrincipal(claimsPrincipal);

        if (userId is not null)
        {
            request.RequestUri = new Uri(request.RequestUri!.ToString() + $"/{userId}");
            return await base.SendAsync(request, cancellationToken);
        }
        else
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
