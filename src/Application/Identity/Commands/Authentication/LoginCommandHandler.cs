using Onion.Application.Common.Errors.Identity;
using Onion.Domain.Entities.Identity;

namespace Onion.Application.Identity.Commands.Authentication;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, IAssertiveResult<LoginResult>>
{
    private readonly IDbContext _dbContext;
    private readonly ISecureHash _secureHash;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IDbContext dbContext,
        ISecureHash secureHash,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _secureHash = secureHash;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<IAssertiveResult<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = _dbContext.Users.Search(request.Username);
        var step1 = VerifyUser(user);
        var step2 = VerifyPassword(step1, request.Password, user?.Salt!, user?.HashedPassword!);
        var step3 = step2.Override<LoginResult>();
        var loginResult = step3.Resolve(_ => {
            var jwtToken = _jwtTokenGenerator.GenerateToken(user!);
            return new LoginResult(user!.Id, user.Username, jwtToken);
        });

        await Task.CompletedTask;
        return loginResult;
    }

    // STEP 1
    private static IResult VerifyUser(User? user)
    {
        return Assertive.Result()
            .Assert(ctx => {
                bool exist = user is not null;
                ctx.Should.Satisfy(exist).WithError(Error.Authentication.UserNotFound);
            });
    }

    // STEP 2
    private IResult VerifyPassword(IResult result, string password, string salt, string hashedPassword)
    {
        return result.Assert(ctx => {
            bool verify = _secureHash.VerifyPassword(password, salt, hashedPassword);
            ctx.Should.Satisfy(verify).WithError(Error.Authentication.IncorrectPassword);
        });
    }
}
