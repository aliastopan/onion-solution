using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Onion.Infrastructure.Authentication;

public static class JwtTokenValidator
{
    public static IServiceCollection AddTokenValidationParameters(this IServiceCollection services,
        IConfiguration configuration)
    {
        var secret = configuration[JwtSettings.Element.Secret];
        var iss = configuration[JwtSettings.Element.Issuer];
        var aud = configuration[JwtSettings.Element.Audience];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        services.AddSingleton(new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = iss,
            ValidAudience = aud,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        });

        return services;
    }
}
