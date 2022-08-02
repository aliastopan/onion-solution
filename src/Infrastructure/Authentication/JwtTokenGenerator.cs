using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Onion.Application.Common.Interfaces;

namespace Onion.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IDateTime _dateTime;

    public JwtTokenGenerator(IDateTime dateTime)
    {
        _dateTime = dateTime;
    }

    public string GenerateToken(Guid id, string username, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super-secret-key"));
        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha384);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role),
        };

        var jwtToken = new JwtSecurityToken(
            issuer: "Onion",
            audience: "Onion",
            expires: _dateTime.UtcNow.AddMinutes(60),
            claims: claims,
            signingCredentials: credential);

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
