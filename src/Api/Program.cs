using Onion.Api.Security;
using Onion.Application;
using Onion.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging((context, logging) =>
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    logging.ClearProviders();
    logging.AddSerilog();
});

builder.Host.ConfigureServices((context, services) =>
{
    services.AddApplicationServices();
    services.AddInfrastructureServices(context.Configuration);
    services.AddEndpoints(typeof(IEndpoint).Assembly);
    services.AddJwtAuthentication();
    services.AddJwtAuthorization();
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseEndpoints();
app.Use(async (context, next) => {
    await next();
    if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        await context.Response.WriteAsync("Request denied.");
});

app.UseAuthentication();
app.UseAuthorization();

app.Run();