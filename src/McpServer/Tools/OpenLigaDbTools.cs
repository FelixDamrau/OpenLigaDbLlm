using System.Collections;
using System.ComponentModel;
using ModelContextProtocol.Server;
using OpenLigaDb.McpServer.Generated;

namespace OpenLigaDb.McpServer.Tools;

[McpServerToolType]
public class OpenLigaDbTools(OpenLigaDbServiceClient client)
{
    private readonly OpenLigaDbServiceClient client = client;

    [McpServerTool(Name = "getAvailableLeagues"), Description("Gets all available leagues")]
    public async Task<ICollection<League>> GetAvailableLeagues() => await client.GetavailableleaguesAsync().ConfigureAwait(false);
}