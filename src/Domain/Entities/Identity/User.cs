using Onion.Domain.Enums;

namespace Onion.Domain.Entities.Identity;

public class User
{
    public User(string username, string email, string hashedPassword, string salt)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        Role = UserRole.Standard;
        IsVerified = false;
        HashedPassword = hashedPassword;
        Salt = salt;
    }

    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public bool IsVerified { get; set; }
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
}
