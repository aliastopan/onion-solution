using Onion.Api.Security;
using Onion.Application;
using Onion.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging((context, logging) =>
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .Filter.ByExcluding(x => x.MessageTemplate.Text.Contains(LogMessage.TokenValidationFailed))
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

app.UseMiddleware<ExceptionGuard>();
app.UseHttpsRedirection();
app.UseEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();