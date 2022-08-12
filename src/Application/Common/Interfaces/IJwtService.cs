using System.Security.Claims;
using Onion.Domain.Entities.Identity;

namespace Onion.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateJwt(User user);
    RefreshToken GenerateRefreshToken(string jwt, User user);
    IResult<(string jwt, string refreshToken)> Refresh(string jwt, string refreshToken);
}
