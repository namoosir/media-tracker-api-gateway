using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Http.Extensions;
using MediaTrackerApiGateway.Models;
using System.Security.Claims;
using MediaTrackerApiGateway.Services.SessionTokenService;

namespace MediaTrackerApiGateway.Middleware;

public class CustomAuthenticationHandler
{
    private readonly ISessionTokenService _sessionTokenService;

    public CustomAuthenticationHandler(ISessionTokenService sessionTokenService)
    {
        _sessionTokenService = sessionTokenService;
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

        if (_sessionTokenService.ValidateToken(token, out ClaimsPrincipal _))
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
