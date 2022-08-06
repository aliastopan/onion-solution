namespace Onion.Contracts.Identity.Authentication;

public record LoginRequest(
    string Username,
    string Password);
