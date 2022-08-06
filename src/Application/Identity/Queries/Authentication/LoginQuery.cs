namespace Onion.Application.Identity.Queries.Authentication;

public record LoginQuery(
    string Username,
    string Password) : IRequest<IAssertiveResult<LoginResult>>;
