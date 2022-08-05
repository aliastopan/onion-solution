using AssertiveResults;
using MediatR;

namespace Onion.Application.Identity.Queries.Authentication;

public record AuthQuery(
    string Username,
    string Password) : IRequest<IAssertiveResult<AuthResult>>;
