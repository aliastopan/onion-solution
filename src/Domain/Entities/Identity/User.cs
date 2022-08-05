namespace Onion.Domain.Entities.Identity;

public class User
{
    public User(string username, string email, string role, string hashedPassword, string salt)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        Role = role;
        IsVerified = false;
        HashedPassword = hashedPassword;
        Salt = salt;
    }

    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsVerified { get; set; }
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
}
