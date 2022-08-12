using Onion.Contracts.Identity.Authentication.Refresh;
using Onion.Application.Identity.Commands.Authentication.Refresh;

namespace Onion.Api.Endpoints.Identity;

public class RefreshEndpoint : IEndpoint
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost("api/refresh", Refresh).AllowAnonymous();
    }

    internal async Task<IResult> Refresh([FromServices] ISender sender,
        HttpContext httpContext)
    {
        var jwt = httpContext.Request.Cookies["jwt"];
        var rwt = httpContext.Request.Cookies["rwt"];
        if(jwt is null || rwt is null)
            return Results.NoContent();

        var command = new RefreshCommand(jwt, rwt);
        var refresh = await sender.Send(command);

        if(refresh.Failed)
        {
            int code = (int)HttpStatusCode.UnprocessableEntity;
            var problemDetails = refresh.ToProblemDetails("api/refresh", code, httpContext);
            return Results.Problem(problemDetails);
        }

        var refreshResponse = refresh.Value.Adapt<RefreshResponse>();
        var cookieOption = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddMinutes(5)
        };
        httpContext.Response.Cookies.Append("jwt", refreshResponse.Jwt, cookieOption);
        httpContext.Response.Cookies.Append("rwt", refreshResponse.RefreshToken, cookieOption);
        return Results.Ok(refreshResponse);
    }
}
