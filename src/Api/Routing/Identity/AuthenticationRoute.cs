using Mapster;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Onion.Api.Extensions;
using Onion.Application.Identity.Authentication;
using Onion.Contracts.Identity.Authentication;

namespace Onion.Api.Routing.Identity;

public class AuthenticationRoute : IRouting
{
    public void MapRoute(WebApplication app)
    {
        app.MapPost("api/auth", Auth);
    }

    internal IResult Auth([FromServices] AuthQuery authQuery,
        HttpContext httpContext, AuthRequest authRequest)
    {
        var authDto = authRequest.Adapt<AuthDto>();
        var authentication = authQuery.Authenticate(authDto);

        if(authentication.Success)
        {
            var authResult = authentication.Value;
            var response = authResult.Adapt<AuthResult>();
            return Results.Ok(response);
        }
        else
        {
            var problemDetails = new ProblemDetails()
            {
                Title = authentication.LastError.Code,
                Detail = authentication.LastError.Description,
                Status = (int)HttpStatusCode.Unauthorized,
                Instance = "/api/auth"
            };
            problemDetails.AddTraceId(httpContext);
            return Results.Problem(problemDetails);
        }
    }
}
