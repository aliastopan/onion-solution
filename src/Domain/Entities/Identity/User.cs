namespace Onion.Domain.Entities.Identity;

public class User
{
    public User(string username, string email, string role, string password, string salt)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        Role = role;
        IsVerified = false;
        Password = password;
        Salt = salt;
    }

    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsVerified { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
}
