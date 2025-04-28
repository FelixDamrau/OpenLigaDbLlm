using OpenLigaDb.McpServer.Generated;
using OpenLigaDb.McpServer.Tools;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("OpenLigaDb", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.OpenTelemetry(o =>
    {
        o.ResourceAttributes = new Dictionary<string, object>()
        { 
            ["service.name"] = "OpenLigaDb.McpServer",
        };
    })
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog(Log.Logger);

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<OpenLigaDbTools>();

builder.Services
    .AddHttpClient()
    .AddSingleton(s => new OpenLigaDbServiceClient("https://api.openligadb.de", s.GetService<HttpClient>()));

var app = builder.Build();

app.MapMcp();

app.Run();
