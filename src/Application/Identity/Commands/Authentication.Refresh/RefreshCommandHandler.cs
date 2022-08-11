
namespace Onion.Application.Identity.Commands.Authentication.Refresh;

public class RefreshCommandHandler
    : IRequestHandler<RefreshCommand, RefreshResult>
{
    private readonly IJwtService _jwtService;

    public RefreshCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task<RefreshResult> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var jwt = _jwtService.RefreshJwt(request.Jwt);
        var refreshResult = new RefreshResult(jwt, Guid.NewGuid().ToString());
        await Task.CompletedTask;
        return refreshResult;
    }
}
