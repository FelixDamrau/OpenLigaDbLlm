using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

// using var tracerProvider = Sdk.CreateTracerProviderBuilder()
//     .AddHttpClientInstrumentation()
//     .AddSource("*")
//     .AddOtlpExporter()
//     .Build();
// using var metricsProvider = Sdk.CreateMeterProviderBuilder()
//     .AddHttpClientInstrumentation()
//     .AddMeter("*")
//     .AddOtlpExporter()
//     .Build();
// using var loggerFactory = LoggerFactory.Create(builder => builder.AddOpenTelemetry(opt => opt.AddOtlpExporter()));

Console.WriteLine("Connecting client to MCP 'OpenLigaDb' server");

var options = new OpenAIClientOptions()
{
    Endpoint = new("https://openrouter.ai/api/v1"),
};
var apiKeyCredential = new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")!);
var openAIClient = new OpenAIClient(apiKeyCredential, options).GetChatClient("openai/gpt-4.1-nano");

// Create a sampling client.
using IChatClient samplingClient = openAIClient.AsIChatClient()
    .AsBuilder()
    // .UseOpenTelemetry(loggerFactory: loggerFactory, configure: o => o.EnableSensitiveData = true)
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
    }
    // ,loggerFactory: loggerFactory
    );

// Get all available tools
Console.WriteLine("Tools available:");
var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"  {tool}");
}

Console.WriteLine();

// Create an IChatClient that can use the tools.
using IChatClient chatClient = openAIClient.AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    // .UseOpenTelemetry(loggerFactory: loggerFactory, configure: o => o.EnableSensitiveData = true)
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
