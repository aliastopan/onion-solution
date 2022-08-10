using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Onion.Application.Common.Interfaces;
using Onion.Domain.Entities.Identity;

namespace Onion.Infrastructure.Authentication;

internal sealed class JwtProvider : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IDateTime _dateTime;
    private readonly IDbContext _dbContext;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public JwtProvider(
        IOptions<JwtSettings> jwtSettings,
        IDateTime dateTime,
        IDbContext dbContext,
        TokenValidationParameters tokenValidationParameters)
    {
        _jwtSettings = jwtSettings.Value;
        _dateTime = dateTime;
        _tokenValidationParameters = tokenValidationParameters;
        _dbContext = dbContext;
    }

    public string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha384);
        var claims = new[]
        {
            new Claim(JwtClaimTypes.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtClaimTypes.Sub, user.Id.ToString()),
            new Claim(JwtClaimTypes.UniqueName, user.Username),
            new Claim(JwtClaimTypes.Role, user.Role.Name),
        };

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwt = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: _dateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
            claims: claims,
            signingCredentials: signingCredentials);

        return jwtHandler.WriteToken(jwt);
    }

    public string RefreshJwt(string jwt)
    {
        var principal = GetPrincipalFromToken(jwt);
        if(principal is null)
            return "Invalid null principal.";

        return GenerateJwt(principal);
    }

    private string GenerateJwt(ClaimsPrincipal principal)
    {
        var userId = principal.Claims.Single(x => x.Type == JwtClaimTypes.Sub).Value;
        var user = _dbContext.Users.Find(Guid.Parse(userId));

        if(user is null)
            return null!;

        return GenerateJwt(user);
    }

    private ClaimsPrincipal GetPrincipalFromToken(string jwtToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(jwtToken, _tokenValidationParameters, out var securityToken);
            if(!HasValidSecurityAlgorithm(securityToken))
                return null!;

            return principal;
        }
        catch
        {
            return null!;
        }
    }

    private static bool HasValidSecurityAlgorithm(SecurityToken securityToken)
    {
        var securityAlgorithm = SecurityAlgorithms.HmacSha384;
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        return jwtSecurityToken is not null && jwtSecurityToken!.Header.Alg
            .Equals(securityAlgorithm, StringComparison.InvariantCultureIgnoreCase);
    }
}
