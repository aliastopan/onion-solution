using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Onion.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ConfigureServices).Assembly);

        return services;
    }
}
