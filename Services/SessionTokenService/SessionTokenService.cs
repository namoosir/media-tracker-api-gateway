using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MediaTrackerApiGateway.Services.SessionTokenService;

public class SessionTokenService : ISessionTokenService
{
    private readonly IConfiguration _configuration;

    public SessionTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // public string GenerateToken(int userId)
    // {
    //     var secret = _configuration["JwtSessionSecretSigningKey"];
    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //     var token = new JwtSecurityToken(
    //         issuer: _configuration["Site:Name"],
    //         audience: _configuration["Client:Web"],
    //         claims: new[]
    //         {
    //             new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
    //             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //         },
    //         expires: DateTime.Now.AddHours(1), // Token expires in 1 hour
    //         signingCredentials: creds
    //     );

    //     return new JwtSecurityTokenHandler().WriteToken(token);
    // }

    public bool ValidateToken(string token, out ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal = null!;

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters(); // Define your validation parameters here

        try
        {
            SecurityToken securityToken;
            claimsPrincipal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out securityToken
            );
            return true;
        }
        catch (Exception ex)
        {
            // Token validation failed
            // You can log or handle the exception as needed
            Console.WriteLine(ex);
            return false;
        }
    }

    public int? GetUserIdFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        var subClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

        if (subClaim != null && int.TryParse(subClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    private TokenValidationParameters GetValidationParameters()
    {
        var secret = _configuration["JwtSessionSecretSigningKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        return new TokenValidationParameters
        {
            // Example parameters (customize as per your needs):
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = true,

            IssuerSigningKey = key,
            ValidIssuer = _configuration["Site:Name"],
            ValidAudience = _configuration["Client:Web"],
        };
    }
}
