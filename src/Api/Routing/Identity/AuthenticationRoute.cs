using System.Net;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Onion.Api.Extensions;
using Onion.Application.Identity.Queries.Authentication;
using Onion.Contracts.Identity.Authentication;

namespace Onion.Api.Routing.Identity;

public class AuthenticationRoute : IRouting
{
    public void MapRoute(WebApplication app)
    {
        app.MapPost("api/auth", Auth);
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
