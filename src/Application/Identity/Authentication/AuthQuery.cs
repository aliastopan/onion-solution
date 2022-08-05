using AssertiveResults;
using Onion.Application.Common.Errors;
using Onion.Application.Common.Interfaces;

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
        var user = _dbContext.Users.FirstOrDefault(x => x.Username == authDto.Username);
        var authResult = Assertive.Result<AuthResult>()
            .Assert(identity =>
            {
                bool exist = user is not null;
                identity.Should.Satisfy(exist).WithError(Error.Authentication.UserNotFound);
            })
            .Assert(password =>
            {
                bool verify = _secureHash.VerifyPassword(authDto.Password, user!.Salt, user.HashedPassword);
                password.Should.Satisfy(verify).WithError(Error.Authentication.IncorrectPassword);
            })
            .Resolve(_ =>
            {
                var accessToken = _jwtTokenGenerator.GenerateToken(user!.Id, user.Username, user.Role);
                return new AuthResult(user!.Id, user.Username, accessToken);
            });

        return authResult;
    }
}
