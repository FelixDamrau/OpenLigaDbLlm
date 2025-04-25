using OpenLigaDb.McpServer.Generated;
using OpenLigaDb.McpServer.Tools;

var builder = WebApplication.CreateBuilder(args);

// builder.Logging.AddConsole(consoleLogOptions =>
// {
//     // Configure all logs to go to stderr
//     consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
// });

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