namespace Onion.Contract.Identity.Registration;

public record RegisterResponse(
    Guid Id,
    string Username,
    string Email,
    string Password,
    string Salt,
    string AccessToken);
