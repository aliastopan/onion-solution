using Onion.Application.Identity.Commands.Registration;
using Onion.Contracts.Identity.Registration;

namespace Onion.Api.Endpoints.Identity;

public class RegistrationEndpoint : IEndpoint
{
    private const string RegisterUri = "/api/register";

    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(RegisterUri, Register).AllowAnonymous();
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
            int code = (int)HttpStatusCode.UnprocessableEntity;
            var problemDetails = registration.ToProblemDetails(RegisterUri, code, httpContext);
            return Results.Problem(problemDetails);
        }
    }
}
