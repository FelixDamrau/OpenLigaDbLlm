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
    public async Task<ICollection<League>> GetAvailableLeagues(CancellationToken cancellationToken)
    {
        logger.Log(LogLevel.Debug, "[MCP Server Tool] Getting all leagues");
        return await client.GetavailableleaguesAsync(cancellationToken).ConfigureAwait(false);
    }

    [McpServerTool(Name = "getFilteredLeagues"), Description("Gets all available leagues that match the given filter")]
    public async Task<ICollection<League>> GetAvailableLeagues(
        [Description("The filter that is used to filter the league name. (Contains, case-insensitve)")] string filter,
        CancellationToken cancellationToken)
    {
        logger.Log(LogLevel.Debug, "[MCP Server Tool] Filtering leagues with filter: {Filter}", filter);
        var leagues = await client
            .GetavailableleaguesAsync(cancellationToken)
            .ConfigureAwait(false);

        return [.. leagues.Where(x => x.LeagueName.Contains(filter, StringComparison.OrdinalIgnoreCase))];
    }

    [McpServerTool(Name = "getAllTeams"), Description("Gets all teams for the given league in the given season")]
    public async Task<ICollection<Team>> GetAllTeams(
        [Description("The league shortcut, like bl1")] string leagueShortcut,
        [Description("The year of the season, e.g. 2024 for the season 2024/25")] int leagueSeason,
        CancellationToken cancellationToken)
    {
        logger.Log(LogLevel.Debug, "[MCP Server Tool] Getting all teams for league/season: {LeagueShortcut}/{LeagueSeason}", leagueShortcut, leagueSeason);
        return await client
            .GetavailableteamsAsync(leagueShortcut, leagueSeason, cancellationToken)
            .ConfigureAwait(false);
    }

    [McpServerTool(Name = "echo"), Description("Echoes the given message.")]
    public string Echo(string message)
    {
        logger.Log(LogLevel.Debug, "[MCP Server Tool] Echoing: {message}", message);
        return $"Echo: {message}";
    }
}
