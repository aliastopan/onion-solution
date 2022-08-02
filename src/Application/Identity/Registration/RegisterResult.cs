namespace Onion.Application.Identity.Registration;

public record RegisterResult(
    Guid Id,
    string Email,
    string Username,
    string Password,
    string Salt,
    string AccessToken);
