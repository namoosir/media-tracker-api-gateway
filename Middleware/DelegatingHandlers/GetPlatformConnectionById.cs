using MediaTrackerApiGateway.Controllers;
using MediaTrackerApiGateway.Data;

namespace MediaTrackerApiGateway.Middleware.DelegatingHandlers;

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
        string token = request.Headers
            .GetValues("Authorization")
            .FirstOrDefault()
            !.Replace("Bearer ", "");
        int userId = await RetrieveUserIdFromDb(token);

        if (userId != -1)
        {
            request.RequestUri = new Uri(request.RequestUri!.ToString() + $"/{userId}");
            return await base.SendAsync(request, cancellationToken);
        }
        else
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
        }
    }

    private async Task<int> RetrieveUserIdFromDb(string token)
    {
        var userInformation = await _userInformationController.GetUserIdByToken(token);

        if (!userInformation.Success || userInformation.Data is null)
        {
            return -1;
        }

        return userInformation.Data.UserId;
    }
}
