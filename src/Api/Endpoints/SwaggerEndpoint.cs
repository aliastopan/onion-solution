namespace Onion.Api.Endpoints;

public class SwaggerEndpoint : IEndpoint, IService
{
    public void DefineEndpoints(WebApplication app)
    {
        if(app.Environment.IsProduction())
            return;

        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}
