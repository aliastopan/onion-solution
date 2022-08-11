namespace Onion.Application.Identity.Commands.Authentication.Refresh;

public record RefreshResult(
    string Jwt,
    string RefreshToken);
