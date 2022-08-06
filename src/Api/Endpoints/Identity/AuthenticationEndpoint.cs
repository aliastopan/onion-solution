using Onion.Application.Identity.Queries.Authentication;
using Onion.Contracts.Identity.Authentication;

namespace Onion.Api.Endpoints.Identity;

public class AuthenticationEndpoint : IEndpoint
{
    private const string LoginUri = "api/login";

    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(LoginUri, Login);
    }

    internal async Task<IResult> Login([FromServices] ISender sender,
        LoginRequest loginRequest, HttpContext httpContext)
    {
        var query = loginRequest.Adapt<LoginQuery>();
        var authentication = await sender.Send(query);

        if(authentication.Success)
        {
            var loginResult = authentication.Value;
            var response = loginResult.Adapt<LoginResult>();
            return Results.Ok(response);
        }
        else
        {
            int code = (int)HttpStatusCode.UnprocessableEntity;
            var problemDetails = authentication.ToProblemDetails(LoginUri, code, httpContext);
            return Results.Problem(problemDetails);
        }
    }
}
