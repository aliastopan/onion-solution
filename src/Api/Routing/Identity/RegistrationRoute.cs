using Mapster;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Onion.Api.Extensions;
using Onion.Application.Identity.Registration;
using Onion.Contract.Identity.Registration;

namespace Onion.Api.Routing.Identity;

public class RegistrationRoute : IRouting
{
    public void MapRoute(WebApplication app)
    {
        app.MapPost("/api/register", Register);
    }

    internal IResult Register([FromServices] RegisterCommand registerCommand,
        RegisterRequest request, HttpContext httpContext)
    {
        var registerDto = request.Adapt<RegisterDto>();
        var registration = registerCommand.Register(registerDto);

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
