using Onion.Api;
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

builder.Host.ConfigureServices((_, services) =>
{
    services.AddEndpointDefinitions(typeof(IEndpointDefinition).Assembly);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseEndpointDefinitions();

app.Run();