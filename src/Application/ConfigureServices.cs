using Microsoft.Extensions.DependencyInjection;

namespace Onion.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }
}