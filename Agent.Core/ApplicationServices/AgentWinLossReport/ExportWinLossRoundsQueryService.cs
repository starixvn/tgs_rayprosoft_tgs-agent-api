using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.PlayerService.Client;
using Tgs.Report.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    ExportWinLossRoundsQueryService : IGetService<ExportWinLossRoundsQuery,
        Stream>
{
    private readonly IAgentClient _agentClient;
    private readonly IReportClient _reportClient;
    private readonly IPlayerServiceClient _playerClient;

    public ExportWinLossRoundsQueryService(
        IReportClient reportClient,
        IPlayerServiceClient playerClient,
        IAgentClient agentClient)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
        _playerClient = playerClient ?? throw new ArgumentNullException(nameof(playerClient));
        _agentClient = agentClient ?? throw new ArgumentNullException(nameof(agentClient));
    }

    public async ValueTask<Stream> GetAsync(
        ExportWinLossRoundsQuery request,
        CancellationToken cancellationToken = default)
    {
        var playerProperties =
            await _playerClient.GetPlayerPropertiesAsync(request.PlayerCode,
                cancellationToken).ConfigureAwait(false);

        _ = Guid.TryParse(playerProperties["AgentId"], out var agentId);

        var childAgents =
            _agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var inAgentLine =
            await childAgents.AnyAsync(x => x.AgentId == agentId || x.ParentAgentId == agentId, cancellationToken)
                .ConfigureAwait(false);

        return await _reportClient.ExportRoundsAsync(new(
                BrandId: request.BrandId,
                inAgentLine ? null : "notInAgentLine",
                PlayerCode: request.PlayerCode,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                request.ClosedFrom,
                request.ClosedTo)).ConfigureAwait(false);
    }
}