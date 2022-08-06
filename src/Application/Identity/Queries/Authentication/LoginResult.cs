namespace Onion.Application.Identity.Queries.Authentication;

public record LoginResult(
    Guid Id,
    string Username,
    string AccessToken);
