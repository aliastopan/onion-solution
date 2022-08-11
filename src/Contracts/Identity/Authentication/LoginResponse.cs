namespace Onion.Contracts.Identity.Authentication;

public record LoginResponse(
    Guid Id,
    string Username,
    string Jwt,
    string RefreshToken);
