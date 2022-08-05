using AssertiveResults;
using Onion.Application.Common.Errors;
using Onion.Application.Common.Interfaces;
using Onion.Domain.Entities.Identity;

namespace Onion.Application.Identity.Authentication;

public class AuthQuery
{
    private readonly IDbContext _dbContext;
    private readonly ISecureHash _secureHash;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthQuery(IDbContext dbContext, ISecureHash secureHash, IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _secureHash = secureHash;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public IAssertiveResult<AuthResult> Authenticate(AuthDto authDto)
    {
        var user = _dbContext.Users.Search(authDto.Username);
        var step1 = VerifyUser(user);
        var step2 = VerifyPassword(step1, authDto.Password, user?.Salt!, user?.HashedPassword!);
        var step3 = step2.Override<AuthResult>();
        var authResult = step3.Resolve(_ => {
            var accessToken = _jwtTokenGenerator.GenerateToken(user!.Id, user.Username, user.Role);
            return new AuthResult(user!.Id, user.Username, accessToken);
        });

        return authResult;
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
