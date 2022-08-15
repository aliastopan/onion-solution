namespace Onion.Application.Identity.Commands.Authentication.Refresh;

public record RefreshCommandResult(
    string Jwt,
    string RefreshToken);
