using System.Reflection;

namespace Onion.Api.Extensions;

internal static class EndpointExtensions
{
    internal static IServiceCollection AddEndpoints(this IServiceCollection services, params Assembly[] assemblies)
    {
        var endpoints = new List<IEndpoint>();

        foreach (var assembly in assemblies)
        {
            endpoints.AddRange(assembly.ExportedTypes
                .Where(x => typeof(IEndpoint).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IEndpoint>()
            );
        }

        services.AddSingleton(endpoints as IReadOnlyCollection<IEndpoint>);

        return services;
    }

    internal static WebApplication UseEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IReadOnlyCollection<IEndpoint>>();

        foreach(var endpoint in endpoints)
        {
            endpoint.DefineEndpoints(app);
        }

        return app;
    }
}