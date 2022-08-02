namespace Onion.Application.Identity.Registration;

public record RegisterDto(
    string Username,
    string Email,
    string Password);
