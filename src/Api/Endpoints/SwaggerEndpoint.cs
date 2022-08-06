namespace Onion.Api.Endpoints;

public class SwaggerEndpoint : IEndpoint
{
    public void DefineEndpoints(WebApplication app)
    {
        if(app.Environment.IsProduction())
            return;

        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
