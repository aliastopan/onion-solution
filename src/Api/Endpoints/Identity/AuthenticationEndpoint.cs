using Onion.Application.Identity.Queries.Authentication;
using Onion.Contracts.Identity.Authentication;

namespace Onion.Api.Endpoints.Identity;

public class AuthenticationEndpoint : IEndpoint
{
    private const string API_AUTH = "api/auth";

    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(API_AUTH, Auth);
    }

    internal async Task<IResult> Auth([FromServices] ISender sender,
        AuthRequest authRequest, HttpContext httpContext)
    {
        var query = authRequest.Adapt<AuthQuery>();
        var authentication = await sender.Send(query);

        if(authentication.Success)
        {
            var authResult = authentication.Value;
            var response = authResult.Adapt<AuthResult>();
            return Results.Ok(response);
        }
        else
        {
            int code = (int)HttpStatusCode.UnprocessableEntity;
            var problemDetails = authentication.ToProblemDetails(API_AUTH, code, httpContext);
            return Results.Problem(problemDetails);
        }
    }
}
