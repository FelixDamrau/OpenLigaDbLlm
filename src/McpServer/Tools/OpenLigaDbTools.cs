using System.ComponentModel;
using ModelContextProtocol.Server;
using OpenLigaDb.McpServer.Generated;

namespace OpenLigaDb.McpServer.Tools;

[McpServerToolType]
public class OpenLigaDbTools(OpenLigaDbServiceClient client, ILogger<OpenLigaDbTools> logger)
{
    private readonly OpenLigaDbServiceClient client = client;
    private readonly ILogger<OpenLigaDbTools> logger = logger;

    [McpServerTool(Name = "getAvailableLeagues"), Description("Gets all available leagues")]
    public async Task<ICollection<League>> GetAvailableLeagues()
    {
        logger.Log(LogLevel.Debug, "[MCP Server Tool] Getting all leagues");
        return await client.GetavailableleaguesAsync().ConfigureAwait(false);
    }

    [McpServerTool(Name = "getFilteredLeagues"), Description("Gets all available leagues that match the given filter")]
    public async Task<ICollection<League>> GetAvailableLeagues(string filter)
    {
        logger.Log(LogLevel.Debug, "[MCP Server Tool] Filtering leagues with filter: {Filter}", filter);
        var leagues = await client
            .GetavailableleaguesAsync()
            .ConfigureAwait(false);

        return [.. leagues.Where(x => x.LeagueName.Contains(filter, StringComparison.OrdinalIgnoreCase))];
    }

    [McpServerTool(Name = "echo"), Description("Echoes the given message.")]
    public string Echo(string message)
    {
        logger.Log(LogLevel.Debug, "[MCP Server Tool] Echoing: {message}", message);
        return $"Echo: {message}";
    }
}
