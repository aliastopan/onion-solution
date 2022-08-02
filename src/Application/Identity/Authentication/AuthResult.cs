namespace Onion.Application.Identity.Authentication;

public record AuthResult(
    Guid Id,
    string Username,
    string AccessToken);