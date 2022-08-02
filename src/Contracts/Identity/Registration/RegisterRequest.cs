namespace Onion.Contract.Identity.Registration;

public record RegisterRequest(
    string Username,
    string Email,
    string Password);
