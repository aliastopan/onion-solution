using Onion.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices((_, services) =>
{
    services.AddEndpointDefinitions(typeof(IEndpointDefinition).Assembly);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseEndpointDefinitions();

app.Run();