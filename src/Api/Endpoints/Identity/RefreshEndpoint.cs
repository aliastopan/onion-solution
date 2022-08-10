using Onion.Contracts.Identity.Authentication.Refresh;
using Onion.Application.Identity.Commands.Authentication.Refresh;
using Serilog;

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
        if(jwt is null)
            return Results.NoContent();

        Log.Logger.Warning("JWT: {jwt}", jwt);
        var command = new RefreshCommand(jwt);
        var refreshResult = await sender.Send(command);

        var refreshResponse = refreshResult.Adapt<RefreshResponse>();
        return Results.Ok(refreshResponse);
    }
}
