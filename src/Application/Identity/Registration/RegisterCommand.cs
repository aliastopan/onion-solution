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
        var step1 = ValidateDto(registerDto);
        var step2 = ValidateUsername(step1, registerDto.Username);
        var step3 = ValidateEmail(step2, registerDto.Email);
        var step4 = step3.Override<RegisterResult>();
        var registerResult = step4.Resolve(_ => {
            var user = CreateUser(registerDto);
            var accessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Username, user.Role);

            _dbContext.Users.Add(user);
            _dbContext.Commit();

            return new RegisterResult(
                user.Id,
                user.Username,
                user.Email,
                user.HashedPassword,
                user.Salt,
                accessToken);
        });

        return registerResult;
    }

    // STEP 1
    private static IResult ValidateDto(RegisterDto registerDto)
    {
        return Assertive.Result()
            .Assert(dto => dto.RegularExpression.Validate(registerDto.Username).Format.Username())
            .Assert(dto => dto.RegularExpression.Validate(registerDto.Password).Format.StrongPassword())
            .Assert(dto => dto.RegularExpression.Validate(registerDto.Email).Format.EmailAddress());
    }

    // STEP 2
    private IResult ValidateUsername(IResult result, string username)
    {
        return result.Assert(ctx => {
            var userQuery = _dbContext.Users.FirstOrDefault(x => x.Username == username);
            var available = userQuery is null;
            ctx.Should.Satisfy(available).WithError(Error.Registration.UsernameTaken);
        });
    }

    // STEP 3
    private IResult ValidateEmail(IResult result, string email)
    {
        return result.Assert(ctx => {
            var emailQuery = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            var available = emailQuery is null;
            ctx.Should.Satisfy(available).WithError(Error.Registration.EmailInUse);
        });
    }

    private User CreateUser(RegisterDto dto)
    {
        string password = _secureHash.HashPassword(dto.Password, out string salt);
        string role = "standard";
        return new User(dto.Username, dto.Email, role, password, salt);
    }
}
