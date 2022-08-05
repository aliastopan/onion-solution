using System.Net;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Onion.Api.Extensions;
using Onion.Application.Identity.Commands.Registration;
using Onion.Contracts.Identity.Registration;

namespace Onion.Api.Routing.Identity;

public class RegistrationRoute : IRouting
{
    public void MapRoute(WebApplication app)
    {
        app.MapPost("/api/register", Register);
    }

    internal async Task<IResult> Register([FromServices] ISender sender,
        RegisterRequest request, HttpContext httpContext)
    {
        var command = request.Adapt<RegisterCommand>();
        var registration = await sender.Send(command);

        if(registration.Success)
        {
            var registerResult = registration.Value;
            var response = registerResult.Adapt<RegisterResponse>();
            return Results.Ok(response);
        }
        else
        {
            var problemDetails = new ProblemDetails()
            {
                Title = registration.LastError.Code,
                Detail = registration.LastError.Description,
                Status = (int)HttpStatusCode.UnprocessableEntity,
                Instance = "/api/register"
            };
            problemDetails.AddTraceId(httpContext);
            return Results.Problem(problemDetails);
        }
    }
}
