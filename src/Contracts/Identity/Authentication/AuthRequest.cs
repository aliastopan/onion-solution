namespace Onion.Contract.Identity.Authentication;

public record AuthRequest(
    string Username,
    string Password);
