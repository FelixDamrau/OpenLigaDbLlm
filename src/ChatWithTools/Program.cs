using System.ClientModel;
using System.ClientModel.Primitives;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using OpenAI;
using OpenLigaDb.Client;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var serviceName = "OpenLigaDb.Chat";
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceInstanceId: Environment.MachineName);

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddHttpClientInstrumentation()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("*")
    .AddOtlpExporter()
    .Build();
using var metricsProvider = Sdk.CreateMeterProviderBuilder()
    .AddHttpClientInstrumentation()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("*")
    .AddOtlpExporter()
    .Build();
using var loggerFactory = LoggerFactory.Create(builder => builder.AddOpenTelemetry(opt =>
{
    opt.AddOtlpExporter();
    opt.SetResourceBuilder(resourceBuilder);
}));

Console.WriteLine("Connecting client to MCP 'OpenLigaDb' server");

var openRouterApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")
    ?? throw new InvalidOperationException("Open router API key is missing!");
var heliconeApiKey = Environment.GetEnvironmentVariable("HELICONE_API_KEY")
    ?? throw new InvalidOperationException("Helicone API key is missing!");

var options = new OpenAIClientOptions()
{
    Endpoint = new("https://openrouter.helicone.ai/api/v1"),
};

// We need this header for the helicone auth
options.AddPolicy(new CustomHeaderPolicy("Helicone-Auth", $"Bearer {heliconeApiKey}"), PipelinePosition.PerCall);
// We use these headers to propagate the app name to OpenRouter
options.AddPolicy(new CustomHeaderPolicy("X-Title", "OpenLigaDbLlm"), PipelinePosition.PerCall);
options.AddPolicy(new CustomHeaderPolicy("HTTP-Referer", "https://github.com/FelixDamrau/OpenLigaDbLlm"), PipelinePosition.PerCall);

var apiKeyCredential = new ApiKeyCredential(openRouterApiKey);
var openAIClient = new OpenAIClient(apiKeyCredential, options).GetChatClient("openai/gpt-4.1-nano");

using IChatClient samplingClient = openAIClient.AsIChatClient()
    .AsBuilder()
    .UseOpenTelemetry(loggerFactory: loggerFactory, configure: o => o.EnableSensitiveData = true)
    .Build();

var transport = new SseClientTransport(new() 
{
    Endpoint = new Uri($"http://localhost:5000/sse"),
});

var mcpClient = await McpClientFactory.CreateAsync(
    transport,
    clientOptions: new()
    {
        Capabilities = new() { Sampling = new() { SamplingHandler = samplingClient.CreateSamplingHandler() } },
    },
    loggerFactory
    );


Console.WriteLine("Tools available:");
var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"  {tool}");
}

Console.WriteLine();

using var chatClient = openAIClient.AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .UseOpenTelemetry(loggerFactory, configure: o => o.EnableSensitiveData = true)
    .Build();

// Have a conversation, making all tools available to the LLM.
List<ChatMessage> messages = [];
while (true)
{
    Console.Write("Q: ");
    messages.Add(new(ChatRole.User, Console.ReadLine()));

    List<ChatResponseUpdate> updates = [];
    await foreach (var update in chatClient.GetStreamingResponseAsync(messages, new() { Tools = [.. tools] }))
    {
        Console.Write(update);
        updates.Add(update);
    }
    Console.WriteLine();

    messages.AddMessages(updates);
}
