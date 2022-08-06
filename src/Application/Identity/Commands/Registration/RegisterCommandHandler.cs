using Onion.Application.Common.Errors.Identity;
using Onion.Domain.Entities.Identity;

namespace Onion.Application.Identity.Commands.Registration;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, IAssertiveResult<RegisterResult>>
{
    private readonly IDbContext _dbContext;
    private readonly ISecureHash _secureHash;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(
        IDbContext dbContext,
        ISecureHash secureHash,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _secureHash = secureHash;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<IAssertiveResult<RegisterResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var step1 = ValidateRequest(request);
        var step2 = ValidateUsername(step1, request.Username);
        var step3 = ValidateEmail(step2, request.Email);
        var step4 = step3.Override<RegisterResult>();
        var registerResult = step4.Resolve(_ => {
            var user = CreateUser(request);
            var accessToken = _jwtTokenGenerator.GenerateToken(user);

            _dbContext.Users.Add(user);
            _dbContext.Commit();

            return new RegisterResult(
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.HashedPassword,
                user.Salt,
                accessToken);
        });

        await Task.CompletedTask;
        return registerResult;
    }

    // STEP 1
    private static IResult ValidateRequest(RegisterCommand request)
    {
        return Assertive.Result()
            .Assert(dto => dto.RegularExpression.Validate(request.Username).Format.Username())
            .Assert(dto => dto.RegularExpression.Validate(request.Password).Format.StrongPassword())
            .Assert(dto => dto.RegularExpression.Validate(request.Email).Format.EmailAddress());
    }

    // STEP 2
    private IResult ValidateUsername(IResult result, string username)
    {
        return result.Assert(ctx => {
            var searchResult = _dbContext.Users.Search(username);
            var available = searchResult is null;
            ctx.Should.Satisfy(available).WithError(Error.Registration.UsernameTaken);
        });
    }

    // STEP 3
    private IResult ValidateEmail(IResult result, string email)
    {
        return result.Assert(ctx => {
            var searchResult = _dbContext.Users.SearchByEmail(email);
            var available = searchResult is null;
            ctx.Should.Satisfy(available).WithError(Error.Registration.EmailInUse);
        });
    }

    private User CreateUser(RegisterCommand request)
    {
        string password = _secureHash.HashPassword(request.Password, out string salt);
        string role = "standard";
        return new User(request.Username, request.Email, role, password, salt);
    }
}
