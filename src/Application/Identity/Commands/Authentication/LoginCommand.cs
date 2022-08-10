namespace Onion.Application.Identity.Commands.Authentication;

public record LoginCommand(
    string Username,
    string Password) : IRequest<IAssertiveResult<LoginResult>>;
