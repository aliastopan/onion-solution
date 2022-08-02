using System.Reflection;

namespace Onion.Api;

internal static class EndpointDefinitionExtensions
{
    internal static IServiceCollection AddEndpointDefinitions(this IServiceCollection services, params Assembly[] assemblies)
    {
        var endpointDefinition = new List<IEndpointDefinition>();

        foreach (var assembly in assemblies)
        {
            endpointDefinition.AddRange(assembly.ExportedTypes
                .Where(x => typeof(IEndpointDefinition).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IEndpointDefinition>()
            );
        }

        services.AddSingleton(endpointDefinition as IReadOnlyCollection<IEndpointDefinition>);

        return services;
    }

    internal static WebApplication UseEndpointDefinitions(this WebApplication app)
    {
        var endpointDefinition = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();

        foreach(var endpoint in endpointDefinition)
        {
            endpoint.DefineEndpoint(app);
        }

        return app;
    }
}