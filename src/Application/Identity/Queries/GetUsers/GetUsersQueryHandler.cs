using Onion.Application.Common.Errors.Identity;

namespace Onion.Application.Identity.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IResult<GetUsersQueryResult>>
{
    private readonly IDbContext _dbContext;

    public GetUsersQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult<GetUsersQueryResult>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _dbContext.Users.GetAll();
        var result = Assertive.Result<GetUsersQueryResult>()
            .Assert(ctx => ctx.Should.NotEmpty(users).WithError(Error.User.NotFound))
            .Resolve(_ => new GetUsersQueryResult(users));

        await Task.CompletedTask;
        return result;
    }
}
