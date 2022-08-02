using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Common.Interfaces;
using Onion.Infrastructure.Persistence;

namespace Onion.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IDbContext, ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase(nameof(ApplicationDbContext));
        });

        return services;
    }
}