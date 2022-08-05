namespace Onion.Application.Identity.Queries.Authentication;

public record AuthResult(
    Guid Id,
    string Username,
    string AccessToken);
