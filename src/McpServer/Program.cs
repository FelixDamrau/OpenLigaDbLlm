using OpenLigaDb.McpServer.Generated;
using OpenLigaDb.McpServer.Tools;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
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
