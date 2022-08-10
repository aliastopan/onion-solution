namespace Onion.Application.Identity.Commands.Registration;

public record RegisterResult(
    Guid Id,
    string Username,
    string Email,
    string Role,
    string Password,
    string Salt,
    string Jwt);
