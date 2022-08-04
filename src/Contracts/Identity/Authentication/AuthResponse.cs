namespace Onion.Contracts.Identity.Authentication;

public record AuthResponse(
    Guid Id,
    string Username,
    string AccessToken);
