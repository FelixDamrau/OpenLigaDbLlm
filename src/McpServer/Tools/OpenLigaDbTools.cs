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
        using (Serilog.Context.LogContext.PushProperty("tool.name", "getAvailableLeagues"))
        {
            logger.Log(LogLevel.Debug, "[MCP Server Tool] Getting all leagues");
        }
        return await client.GetavailableleaguesAsync(cancellationToken).ConfigureAwait(false);
    }

    [McpServerTool(Name = "getFilteredLeagues"), Description("Gets all available leagues that match the given filter")]
    public async Task<ICollection<League>> GetAvailableLeagues(
        [Description("The filter that is used to filter the league name. (Contains, case-insensitve)")] string filter,
        CancellationToken cancellationToken)
    {
        using (Serilog.Context.LogContext.PushProperty("tool.name", "getFilteredLeagues"))
        {
            logger.Log(LogLevel.Debug, "[MCP Server Tool] Filtering leagues with filter: {Filter}", filter);
        }
        var leagues = await client
            .GetavailableleaguesAsync(cancellationToken)
            .ConfigureAwait(false);

        return [.. leagues.Where(l => l.LeagueName.Contains(filter, StringComparison.OrdinalIgnoreCase))];
    }

    [McpServerTool(Name = "getAllTeams"), Description("Gets all teams for the given league in the given season")]
    public async Task<ICollection<Team>> GetAllTeams(
        [Description("The league shortcut, like bl1")] string leagueShortcut,
        [Description("The year of the season, e.g. 2024 for the season 2024/25")] int leagueSeason,
        CancellationToken cancellationToken)
    {
        using (Serilog.Context.LogContext.PushProperty("tool.name", "getAllTeams"))
        {
            logger.Log(LogLevel.Debug, "[MCP Server Tool] Getting all teams for league/season: {LeagueShortcut}/{LeagueSeason}", leagueShortcut, leagueSeason);
        }
        return await client
            .GetavailableteamsAsync(leagueShortcut, leagueSeason, cancellationToken)
            .ConfigureAwait(false);
    }

    [McpServerTool(Name = "getMatchesByLeagueAndTeam"), Description("Gets all teams for the given league in the given season")]
    public async Task<ICollection<Match>> GetMatches(
        [Description("The league shortcut, like bl1")] string leagueShortcut,
        [Description("The year of the season, e.g. 2024 for the season 2024/25")] int leagueSeason,
        [Description("A filter string for the team in question (case sensitive), e.g. Bayern for FC Bayern München.")] string teamFilter,
        CancellationToken cancellationToken)
    {
        using (Serilog.Context.LogContext.PushProperty("tool.name", "getMatchesByLeagueAndTeam"))
        {
            logger.Log(
                LogLevel.Debug,
                "[MCP Server Tool] Getting all teams for league/season/team filter: {LeagueShortcut}/{LeagueSeason}/{teamFilter}", leagueShortcut, leagueSeason, teamFilter);
        }
        return await client
            .GetmatchdataAll3Async(leagueShortcut, leagueSeason, teamFilter, cancellationToken)
            .ConfigureAwait(false);
    }

    [McpServerTool(Name = "getMatchesByLeagueAndDay"), Description("Gets all matches of the given day of a given league.")]
    public async Task<ICollection<Match>> GetMatches(
        [Description("The league shortcut, like bl1")] string leagueShortcut,
        [Description("The year of the season, e.g. 2024 for the season 2024/25")] int leagueSeason,
        [Description("The match day of the league")] int matchDay,
        CancellationToken cancellationToken)
    {
        using (Serilog.Context.LogContext.PushProperty("tool.name", "getMatchesByLeagueAndDay"))
        {
            logger.Log(
                LogLevel.Debug,
                "[MCP Server Tool] Getting all teams for league/season/match day: {LeagueShortcut}/{LeagueSeason}/{matchDay}", leagueShortcut, leagueSeason, matchDay);
        }
        return await client
            .GetmatchdataAllAsync(leagueShortcut, leagueSeason, matchDay, cancellationToken)
            .ConfigureAwait(false);
    }

    [McpServerTool(Name = "echo"), Description("Echoes the given message.")]
    public string Echo(string message)
    {
        using (Serilog.Context.LogContext.PushProperty("tool.name", "echo"))
        {
            logger.Log(LogLevel.Debug, "[MCP Server Tool] Echoing: {message}", message);
        }
        return $"Echo: {message}";
    }
}
