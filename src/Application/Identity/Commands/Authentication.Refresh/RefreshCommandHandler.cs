
namespace Onion.Application.Identity.Commands.Authentication.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, IResult<RefreshCommandResult>>
{
    private readonly IJwtService _jwtService;

    public RefreshCommandHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public async Task<IResult<RefreshCommandResult>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var step1 = _jwtService.Refresh(request.Jwt, request.RefreshToken);
        var step2 = step1.Override<RefreshCommandResult>(out var token);
        var result = step2.Resolve(_ =>
        {
            return new RefreshCommandResult(token.jwt, token.refreshToken);
        });

        await Task.CompletedTask;
        return result;
    }
}
