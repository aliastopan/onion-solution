using System.Reflection;

namespace Onion.Api;

internal static class EndpointDefinitionExtensions
{
    internal static IServiceCollection AddEndpointRouting(this IServiceCollection services, params Assembly[] assemblies)
    {
        var routes = new List<IRouting>();

        foreach (var assembly in assemblies)
        {
            routes.AddRange(assembly.ExportedTypes
                .Where(x => typeof(IRouting).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IRouting>()
            );
        }

        services.AddSingleton(routes as IReadOnlyCollection<IRouting>);

        return services;
    }

    internal static WebApplication UseRouteEndpoint(this WebApplication app)
    {
        var routes = app.Services.GetRequiredService<IReadOnlyCollection<IRouting>>();

        foreach(var route in routes)
        {
            route.MapRoute(app);
        }

        return app;
    }
}