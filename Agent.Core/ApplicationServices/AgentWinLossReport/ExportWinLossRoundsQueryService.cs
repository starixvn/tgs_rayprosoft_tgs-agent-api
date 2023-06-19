using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tgs.PlayerService.Client;
using Tgs.Report.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    ExportWinLossRoundsQueryService : IGetService<ExportWinLossRoundsQuery,
        Stream>
{
    private readonly IReportClient _reportClient;
    private readonly IPlayerServiceClient _playerClient;

    public ExportWinLossRoundsQueryService(
        IReportClient reportClient,
        IPlayerServiceClient playerClient)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
        _playerClient = playerClient ?? throw new ArgumentNullException(nameof(playerClient));
    }

    public async ValueTask<Stream> GetAsync(
        ExportWinLossRoundsQuery request,
        CancellationToken cancellationToken = default)
    {
        var playerProperties =
            await _playerClient.GetPlayerPropertiesAsync(request.PlayerCode,
                cancellationToken).ConfigureAwait(false);

        return !(Guid.TryParse(playerProperties["AgentId"], out var agentId) && request.AgentId == agentId)
            ? new MemoryStream()
            : await _reportClient.ExportRoundsAsync(new(
            BrandId: request.BrandId,
            null,
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