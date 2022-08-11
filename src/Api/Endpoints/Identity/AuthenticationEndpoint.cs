using Onion.Application.Identity.Commands.Authentication;
using Onion.Contracts.Identity.Authentication;

namespace Onion.Api.Endpoints.Identity;

public class AuthenticationEndpoint : IEndpoint
{
    private const string LoginUri = "api/login";

    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(LoginUri, Login).AllowAnonymous();
    }

    internal async Task<IResult> Login([FromServices] ISender sender,
        LoginRequest loginRequest, HttpContext httpContext)
    {
        var command = loginRequest.Adapt<LoginCommand>();
        var authentication = await sender.Send(command);

        if(authentication.Success)
        {
            var loginResult = authentication.Value;
            var response = loginResult.Adapt<LoginResult>();
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(5)
            };
            httpContext.Response.Cookies.Append("jwt", response.Jwt, cookieOption);
            httpContext.Response.Cookies.Append("rwt", response.RefreshToken, cookieOption);
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
