namespace Onion.Application.Identity.Commands.Authentication;

public record LoginCommandResult(
    Guid Id,
    string Username,
    string Jwt,
    string RefreshToken);
