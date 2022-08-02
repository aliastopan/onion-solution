namespace Onion.Application.Common.Interfaces;

public interface ISecureHash
{
    string GenerateSalt();
    string HashPassword(string password, string salt);
    string HashPassword(string password, out string salt);
    bool VerifyPassword(string password, string salt, string hashedPassword);
}
