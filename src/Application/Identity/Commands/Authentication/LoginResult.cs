namespace Onion.Application.Identity.Commands.Authentication;

public record LoginResult(
    Guid Id,
    string Username,
    string Jwt);
