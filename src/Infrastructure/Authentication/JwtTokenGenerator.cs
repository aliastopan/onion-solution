using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Onion.Application.Common.Interfaces;

namespace Onion.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly IDateTime _dateTime;

    public JwtTokenGenerator(IDateTime dateTime, IOptions<JwtSettings> jwtSettings)
    {
        _dateTime = dateTime;
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(Guid id, string username, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha384);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role),
        };

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: _dateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            claims: claims,
            signingCredentials: credential);

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
