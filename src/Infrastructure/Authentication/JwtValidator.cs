using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Onion.Infrastructure.Authentication;

public static class JwtValidator
{
    public static TokenValidationParameters GetValidationParameters(IConfiguration configuration)
    {
        var secret = configuration[JwtSettings.Element.Secret];
        var iss = configuration[JwtSettings.Element.Issuer];
        var aud = configuration[JwtSettings.Element.Audience];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        return new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = iss,
            ValidAudience = aud,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    }
}
