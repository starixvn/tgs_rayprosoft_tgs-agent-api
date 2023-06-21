using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.AgentWinLoss;
using Tgs.ServiceUtility;
using Tgs.PlayerService.Client;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    ExportAgentWinLossQueryService : IGetService<ExportAgentWinLossQuery, Stream>
{
    private readonly IAgentClient _agentClient;
    private readonly IBrandRepository _brandRepository;
    private readonly IPlayerServiceClient _playerClient;
    private readonly IReportClient _reportClient;

    public ExportAgentWinLossQueryService(
        IReportClient reportClient,
        IBrandRepository brandRepository,
        IAgentClient agentClient,
        IPlayerServiceClient playerClient)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        _agentClient = agentClient ?? throw new ArgumentNullException(nameof(agentClient));
        _playerClient = playerClient ?? throw new ArgumentNullException(nameof(playerClient));
    }

    public async ValueTask<Stream> GetAsync(ExportAgentWinLossQuery request,
        CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetBrandInfoAsync(request.BrandId).ConfigureAwait(false);

        return string.IsNullOrEmpty(request.Username) switch
        {
            true =>
                await ExportDownLineAgentWinLossByMasterAgentIdAsync(request, brand!.PlatformCharge)
                    .ConfigureAwait(false),
            false when request is { DownLine: false, MemberType: AccountType.Agent } =>
                await ExportAgentWinLossAsync(request, brand!.PlatformCharge, cancellationToken)
                    .ConfigureAwait(false),
            false when request is { DownLine: true, MemberType: AccountType.Agent } =>
                await ExportDownLineAgentWinLossAsync(request, brand!.PlatformCharge, cancellationToken)
                    .ConfigureAwait(false),
            false when request.MemberType == AccountType.Player =>
                await ExportPlayerWinLossAsync(request, brand!.PlatformCharge, cancellationToken)
                    .ConfigureAwait(false),
            _ => new MemoryStream()
        };
    }

    private async Task<Stream> ExportAgentWinLossAsync(
        ExportAgentWinLossQuery request,
        decimal platformCharge,
        CancellationToken cancellationToken)
    {
        var childAgents =
            _agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var agent = await _agentClient
            .GetAgentInfoByUsernameAsync(new GetAgentInfoByUsernameRequest(request.BrandId, request.Username),
                cancellationToken).ConfigureAwait(false);

        var subAgent =
            await childAgents.FirstOrDefaultAsync(x => x.AgentId == agent.Id, cancellationToken)
                .ConfigureAwait(false);

        var agentId = request.AgentId == agent?.Id ? request.AgentId : subAgent?.AgentId;

        return await _reportClient.ExportAgentWinLossAsync(new(request.BrandId,
            agentId ?? Guid.Empty,
            platformCharge,
            request.From,
            request.To)).ConfigureAwait(false);
    }

    private async Task<Stream>
        ExportDownLineAgentWinLossByMasterAgentIdAsync(
            ExportAgentWinLossQuery request,
            decimal platformCharge)
    {
        return await _reportClient.ExportDownLineAgentWinLossAsync(new(request.BrandId,
            request.AgentId,
            platformCharge,
            request.From,
            request.To)).ConfigureAwait(false);
    }

    private async Task<Stream>
        ExportDownLineAgentWinLossAsync(
            ExportAgentWinLossQuery request,
            decimal platformCharge,
            CancellationToken cancellationToken)
    {
        var childAgents =
            _agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var agent = await _agentClient.GetAgentInfoByUsernameAsync(
            new GetAgentInfoByUsernameRequest(request.BrandId, request.Username),
            cancellationToken).ConfigureAwait(false);

        var subAgent =
            await childAgents.FirstOrDefaultAsync(x => x.AgentId == agent.Id, cancellationToken)
                .ConfigureAwait(false);

        var agentId = request.AgentId == agent?.Id ? request.AgentId : subAgent?.AgentId;

        return await _reportClient
            .ExportDownLineAgentWinLossAsync(new(request.BrandId,
                agentId ?? Guid.Empty,
                platformCharge,
                request.From,
                request.To)).ConfigureAwait(false);
    }

    private async Task<Stream> ExportPlayerWinLossAsync(
        ExportAgentWinLossQuery request,
        decimal platformCharge,
        CancellationToken cancellationToken)
    {
        var childAgents =
            _agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var player =
            await _playerClient.GetPlayerInfoByUserNameAsync(request.BrandId,
                request.Username,
                cancellationToken).ConfigureAwait(false);

        if (player == null)
        {
            return await _reportClient
                .ExportPlayerWinLossAsync(new ExportPlayerWinLossRequest(
                    "notInAgentLine",
                    platformCharge,
                    request.From,
                    request.To)).ConfigureAwait(false);
        }

        var playerProperties =
            await _playerClient.GetPlayerPropertiesAsync(player.PlayerCode,
                cancellationToken).ConfigureAwait(false);

        _ = Guid.TryParse(playerProperties["AgentId"], out var agentId);

        var inAgentLine = await childAgents
            .AnyAsync(x => x.AgentId == agentId || x.ParentAgentId == agentId, cancellationToken)
            .ConfigureAwait(false);

        return await _reportClient
            .ExportPlayerWinLossAsync(new ExportPlayerWinLossRequest(
                inAgentLine ? player.PlayerCode : "notInAgentLine",
                platformCharge,
                request.From,
                request.To)).ConfigureAwait(false);
    }
}