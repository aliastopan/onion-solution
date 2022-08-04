namespace Onion.Contracts.Identity.Authentication;

public record AuthRequest(
    string Username,
    string Password);
