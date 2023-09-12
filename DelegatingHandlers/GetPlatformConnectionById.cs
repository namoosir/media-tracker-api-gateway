using MediaTrackerApiGateway.Controllers;
using MediaTrackerApiGateway.Data;

namespace MediaTrackerApiGateway.DelegatingHandlers;

public class GetPlatformConnectionById : DelegatingHandler
{
    private readonly UserInformationController _userInformationController;

    public GetPlatformConnectionById(UserInformationController userInformationController)
        : base()
    {
        _userInformationController = userInformationController;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        string? token = request.Headers
            .GetValues("Authorization")
            .FirstOrDefault()
            ?.Replace("Bearer ", "");
        int userId = await RetrieveUserIdFromDb(token);

        if (userId != -1)
        {
            request.RequestUri = new Uri(request.RequestUri.ToString() + $"/{userId}");
            return await base.SendAsync(request, cancellationToken);
        }
        else
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
        }
    }

    private async Task<int> RetrieveUserIdFromDb(string? token)
    {
        if (token is null)
        {
            return -1;
        }

        var userInformation = (await _userInformationController.GetUserIdByToken(token)).Data;

        if (userInformation is null)
        {
            return -1;
        }

        return userInformation.UserId;
    }
}
