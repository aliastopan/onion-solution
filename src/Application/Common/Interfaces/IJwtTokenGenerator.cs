namespace Onion.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid id, string username);
}
