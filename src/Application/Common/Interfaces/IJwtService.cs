using Onion.Domain.Entities.Identity;

namespace Onion.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateJwt(User user);
    string RefreshJwt(string jwt);
    RefreshToken GenerateRefreshToken(string jwt, User user);
}
