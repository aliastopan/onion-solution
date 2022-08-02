using AssertiveResults;
using Onion.Application.Common.Errors;
using Onion.Application.Common.Interfaces;
using Onion.Domain.Entities.Identity;

namespace Onion.Application.Identity.Registration;

public class RegisterCommand
{
    private readonly IDbContext _dbContext;
    private readonly ISecureHash _secureHash;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommand(IDbContext dbContext, ISecureHash secureHash, IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _secureHash = secureHash;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public IAssertiveResult<RegisterResult> Register(RegisterDto registerDto)
    {
        var result = Assertive.Result()
            .Assert(password =>
            {
                password.RegularExpression.Match(registerDto.Password).Validates.PasswordStrength();
            })
            .Break()
            .Assert(user =>
            {
                var userQuery = _dbContext.Users.FirstOrDefault(x => x.Username == registerDto.Username);
                var available = userQuery is null;
                user.Should.Satisfy(available).WithError(Error.Registration.UsernameTaken);
            })
            .Break()
            .Assert(email =>
            {
                var emailQuery =_dbContext.Users.FirstOrDefault(x => x.Email == registerDto.Email);
                var available = emailQuery is null;
                email.Should.Satisfy(available).WithError(Error.Registration.EmailInUse);
            })
            .Resolve(_ =>
            {
                var user = CreateUser(registerDto);
                var accessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Username, user.Role);

                _dbContext.Users.Add(user);
                _dbContext.Commit();

                return new RegisterResult(
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Password,
                    user.Salt,
                    accessToken);
            });

        return result;
    }

    private User CreateUser(RegisterDto dto)
    {
        string password = _secureHash.HashPassword(dto.Password, out string salt);
        string role = "none";
        return new User(dto.Username, dto.Email, role, password, salt);
    }
}
