namespace Onion.Api.Routing;

public class SwaggerRoute : IRouting
{
    public void MapRoute(WebApplication app)
    {
        if(app.Environment.IsProduction())
            return;

        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
