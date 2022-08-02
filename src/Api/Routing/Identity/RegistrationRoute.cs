using Onion.Contract.Identity.Registration;

namespace Onion.Api.Routing.Identity;

public class RegistrationRoute : IRouting
{
    public void MapRoute(WebApplication app)
    {
        app.MapPost("/api/register", Register);
    }

    internal IResult Register(RegisterRequest request)
    {
        var response = new RegisterResponse(
            Guid.NewGuid(),
            request.Username,
            request.Email,
            request.Password,
            "s4lt",
            Guid.NewGuid().ToString());

        return Results.Ok(response);
    }
}
