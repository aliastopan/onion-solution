using Onion.Application.Identity.Commands.Authentication;
using Onion.Application.Identity.Commands.Authentication.Refresh;
using Onion.Application.Identity.Commands.Registration;
using Onion.Contracts.Identity.Authentication;
using Onion.Contracts.Identity.Authentication.Refresh;
using Onion.Contracts.Identity.Registration;

namespace Onion.Api.Endpoints.Identity;

public class IdentityEndpoint : IEndpoint
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapPost(Uri.Identity.Register, Register)
            .AllowAnonymous()
            .Produces<RegisterResponse>()
            .WithTags(Uri.Identity.Tag);

        app.MapPost(Uri.Identity.Login, Login)
            .AllowAnonymous()
            .Produces<LoginResponse>()
            .WithTags(Uri.Identity.Tag);

        app.MapPost(Uri.Identity.Refresh, Refresh)
            .Produces<RefreshResponse>()
            .WithTags(Uri.Identity.Tag);
    }

    internal async Task<IResult> Register([FromServices] ISender sender,
        RegisterRequest request, HttpContext httpContext)
    {
        var command = request.Adapt<RegisterCommand>();
        var registration = await sender.Send(command);

        if(registration.HasFailed)
        {
            var statusCode = (int)HttpStatusCode.UnprocessableEntity;
            var problemDetails = registration.ToProblemDetails(httpContext, statusCode);
            return Results.Problem(problemDetails);
        }

        var registerResult = registration.Value;
        var registerResponse = registerResult.Adapt<RegisterResponse>();
        return Results.Ok(registerResponse);
    }

    internal async Task<IResult> Login([FromServices] ISender sender,
        LoginRequest loginRequest, HttpContext httpContext)
    {
        var command = loginRequest.Adapt<LoginCommand>();
        var authentication = await sender.Send(command);
        if(authentication.HasFailed)
        {
            var statusCode = (int)HttpStatusCode.Unauthorized;
            var problemDetails = authentication.ToProblemDetails(httpContext, statusCode);
            return Results.Problem(problemDetails);
        }

        var loginResult = authentication.Value;
        var loginResponse = loginResult.Adapt<LoginResult>();
        var cookieOption = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddMinutes(5)
        };
        httpContext.Response.Cookies.Append("jwt", loginResponse.Jwt, cookieOption);
        httpContext.Response.Cookies.Append("rwt", loginResponse.RefreshToken, cookieOption);
        return Results.Ok(loginResponse);
    }

    internal async Task<IResult> Refresh([FromServices] ISender sender,
        HttpContext httpContext)
    {
        var jwt = httpContext.Request.Cookies["jwt"];
        var rwt = httpContext.Request.Cookies["rwt"];
        if(jwt is null || rwt is null)
            return Results.NoContent();

        var command = new RefreshCommand(jwt, rwt);
        var refresh = await sender.Send(command);
        if(refresh.HasFailed)
        {
            return Results.Problem(refresh.ToProblemDetails(
                httpContext,
                (int)HttpStatusCode.Unauthorized));
        }

        var refreshResult = refresh.Value;
        var refreshResponse = refreshResult.Adapt<RefreshResponse>();
        var cookieOption = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddMinutes(5)
        };
        httpContext.Response.Cookies.Append("jwt", refreshResponse.Jwt, cookieOption);
        httpContext.Response.Cookies.Append("rwt", refreshResponse.RefreshToken, cookieOption);
        return Results.Ok(refreshResponse);
    }
}
