namespace Onion.Application.Identity.Commands.Authentication.Refresh;

public record RefreshCommand(
    string Jwt) : IRequest<RefreshResult>;
