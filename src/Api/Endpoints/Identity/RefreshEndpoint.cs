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
        var refreshResult = await sender.Send(command);
        var refreshResponse = refreshResult.Adapt<RefreshResponse>();
        var cookieOption = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddMinutes(5)
        };
        httpContext.Response.Cookies.Append("jwt", refreshResponse.Jwt, cookieOption);
        httpContext.Response.Cookies.Append("rwt", refreshResponse.Jwt, cookieOption);
        return Results.Ok(refreshResponse);
    }
}
