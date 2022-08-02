using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Identity.Registration;

namespace Onion.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<RegisterCommand>();

        return services;
    }
}
