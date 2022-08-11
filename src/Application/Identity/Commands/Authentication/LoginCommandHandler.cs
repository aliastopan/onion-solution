using Onion.Application.Common.Errors.Identity;
using Onion.Domain.Entities.Identity;

namespace Onion.Application.Identity.Commands.Authentication;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, IResult<LoginResult>>
{
    private readonly IDbContext _dbContext;
    private readonly ISecureHash _secureHash;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IDbContext dbContext,
        ISecureHash secureHash,
        IJwtService jwtService)
    {
        _dbContext = dbContext;
        _secureHash = secureHash;
        _jwtService = jwtService;
    }

    public async Task<IResult<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.Search(request.Username);
        var step1 = VerifyUser(user);
        var step2 = VerifyPassword(step1, request.Password, user?.Salt!, user?.HashedPassword!);
        var step3 = step2.Override<LoginResult>();
        var loginResult = step3.Resolve(_ => {
            var jwt = _jwtService.GenerateJwt(user!);
            var refreshToken = _jwtService.GenerateRefreshToken(jwt, user!).Token;
            return new LoginResult(user!.Id, user.Username, jwt, refreshToken);
        });

        await Task.CompletedTask;
        return loginResult;
    }

    // STEP 1
    private static ISubject VerifyUser(User? user)
    {
        return Assertive.Result()
            .Assert(ctx => {
                bool exist = user is not null;
                ctx.Should.Satisfy(exist).WithError(Error.Authentication.UserNotFound);
            });
    }

    // STEP 2
    private ISubject VerifyPassword(ISubject subject, string password, string salt, string hashedPassword)
    {
        return subject.Assert(ctx => {
            bool verify = _secureHash.VerifyPassword(password, salt, hashedPassword);
            ctx.Should.Satisfy(verify).WithError(Error.Authentication.IncorrectPassword);
        });
    }
}
