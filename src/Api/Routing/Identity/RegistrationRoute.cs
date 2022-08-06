using Onion.Application.Identity.Commands.Registration;
using Onion.Contracts.Identity.Registration;

namespace Onion.Api.Routing.Identity;

public class RegistrationRoute : IRouting
{
    private const string API_REGISTER = "/api/register";

    public void MapRoute(WebApplication app)
    {
        app.MapPost(API_REGISTER, Register);
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
            var problemDetails = registration.ToProblemDetails(API_REGISTER, code, httpContext);
            return Results.Problem(problemDetails);
        }
    }
}
