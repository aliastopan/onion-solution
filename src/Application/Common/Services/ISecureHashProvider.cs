namespace Onion.Application.Common.Services;

public interface ISecureHashProvider
{
    string GenerateSalt();
    string HashPassword(string password, string salt);
    string HashPassword(string password, out string salt);
    bool VerifyPassword(string password, string salt, string hashedPassword);
}
