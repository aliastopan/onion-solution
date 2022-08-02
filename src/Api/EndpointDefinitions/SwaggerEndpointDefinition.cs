namespace Onion.Api.EndpointDefinitions;

public class SwaggerEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoint(WebApplication app)
    {
        if(app.Environment.IsProduction())
            return;

        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
