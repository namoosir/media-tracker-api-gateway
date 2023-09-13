using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MediaTrackerApiGateway.Controllers;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Http.Extensions;

namespace MediaTrackerApiGateway.Middleware;

public class CustomAuthenticationHandler
{
    private readonly UserInformationController _userInformationController;

    public CustomAuthenticationHandler(UserInformationController userInformationController)
    {
        _userInformationController = userInformationController;
    }

    public async Task HandleAsync(HttpContext context, Func<Task> next)
    {
        string path = context.Request.Path;
        if (path == "/Auth/sign/google")
        {
            //no need to check for anything
            await next.Invoke();
        }

        string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            context.Items.SetError(new UnauthenticatedError("No auth token in header"));
        }

        var userInformation = await _userInformationController.GetUserIdByToken(token);

        if (userInformation.Success)
        {
            await next.Invoke();
        }
        else
        {
            context.Items.SetError(new UnauthenticatedError("Invalid token"));
            return;
        }
    }
}
