namespace Onion.Application.Identity.Commands.Registration;

public record RegisterCommand(
    string Username,
    string Email,
    string Password) : IRequest<IResult<RegisterResult>>;
