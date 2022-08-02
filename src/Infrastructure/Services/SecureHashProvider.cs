using System.Security.Cryptography;
using System.Text;
using Onion.Application.Common.Services;

namespace Onion.Infrastructure.Services;

public class SecureHashProvider : ISecureHashProvider
{
    public string GenerateSalt()
    {
        return CCred.Sauce.GenerateSalt(8);
    }

    public string HashPassword(string password, string salt)
    {
        return CCred.Sauce.GetHash<SHA384>(password, salt, Encoding.UTF8);
    }

    public string HashPassword(string password, out string salt)
    {
        salt = GenerateSalt();
        return CCred.Sauce.GetHash<SHA384>(password, salt, Encoding.UTF8);
    }

    public bool VerifyPassword(string password, string salt, string hashedPassword)
    {
        return CCred.Sauce.Verify<SHA384>(password, salt, hashedPassword, Encoding.UTF8);
    }
}
