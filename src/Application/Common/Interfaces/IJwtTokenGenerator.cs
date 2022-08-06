using Onion.Domain.Entities.Identity;

namespace Onion.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
