using Mapster;
using Onion.Application.Identity.Registration;
using Onion.Contract.Identity.Registration;

namespace Onion.Api.Routing.Identity;

public class RegistrationRoute : IRouting
{
    public void MapRoute(WebApplication app)
    {
        app.MapPost("/api/register", Register);
    }

    internal IResult Register(RegisterCommand registerCommand, RegisterRequest request)
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
            return Results.UnprocessableEntity();
        }
    }
}
