using Onion.Application.Common.Errors.Identity;

namespace Onion.Application.Identity.Management.Queries.GetAllUsers;

public class GetAllUsersQueryHandler
    : IRequestHandler<GetAllUsersQuery, IResult<GetAllUsersResult>>
{
    private readonly IDbContext _dbContext;

    public GetAllUsersQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult<GetAllUsersResult>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _dbContext.Users.GetAll();
        var getAllUsersResult = Assertive.Result<GetAllUsersResult>()
            .Assert(ctx => ctx.Should.NotEmpty(users).WithError(Error.User.NotFound))
            .Resolve(_ => new GetAllUsersResult(users));

        await Task.CompletedTask;
        return getAllUsersResult;
    }
}
