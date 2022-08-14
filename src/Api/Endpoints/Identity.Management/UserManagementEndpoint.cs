using Onion.Application.Identity.Management.Queries.GetAllUsers;

namespace Onion.Api.Endpoints.Identity.Management;

public class UserManagementEndpoint : IEndpoint
{
    private const string GetAllUserUri = "api/user/get-all";

    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet(GetAllUserUri, GetAllUser);
    }

    internal async Task<IResult> GetAllUser([FromServices] ISender sender,
        HttpContext httpContext)
    {
        var query = new GetAllUsersQuery();
        var getAllUser = await sender.Send(query);

        if(getAllUser.IsSuccess)
        {
            var value = getAllUser.Value;
            return Results.Ok(value);
        }
        else
        {
            int code = (int)HttpStatusCode.NotFound;
            var problemDetails = getAllUser.ToProblemDetails(GetAllUserUri, code, httpContext);
            return Results.Problem(problemDetails);
        }
    }
}
