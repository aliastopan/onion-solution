namespace Onion.Contracts.Identity.Registration;

public record RegisterResponse(
    Guid Id,
    string Username,
    string Email,
    string Role,
    string Password,
    string Salt,
    string Jwt);
