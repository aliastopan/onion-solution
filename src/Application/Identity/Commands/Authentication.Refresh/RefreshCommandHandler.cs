
namespace Onion.Application.Identity.Commands.Authentication.Refresh;

public class RefreshCommandHandler
    : IRequestHandler<RefreshCommand, IResult<RefreshResult>>
{
    private readonly IJwtService _jwtService;

    public RefreshCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task<IResult<RefreshResult>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var step1 = _jwtService.Refresh(request.Jwt, request.RefreshToken);
        var step2 = step1.Override<RefreshResult>(out var token);
        var refreshResult = step2.Resolve(_ =>
        {
            return new RefreshResult(token.jwt, token.refreshToken);
        });

        await Task.CompletedTask;
        return refreshResult;
    }
}
