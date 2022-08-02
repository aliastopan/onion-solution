namespace Onion.Contract.Identity.Authentication;

public record AuthResponse(
    Guid Id,
    string Username,
    string AccessToken);
