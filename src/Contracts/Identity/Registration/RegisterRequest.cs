namespace Onion.Contracts.Identity.Registration;

public record RegisterRequest(
    string Username,
    string Email,
    string Password);
