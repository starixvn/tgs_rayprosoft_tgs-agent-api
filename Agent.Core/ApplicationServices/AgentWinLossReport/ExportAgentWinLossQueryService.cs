using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using TGS.Core.Models.Enums;
using Tgs.PlayerService.Client;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.AgentWinLoss;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    ExportAgentWinLossQueryService(
        IReportClient reportClient,
        IAgentClient agentClient,
        IPlayerServiceClient playerClient)
    : IGetService<ExportAgentWinLossQuery, Stream>
{
    public async ValueTask<Stream> GetAsync(ExportAgentWinLossQuery request,
        CancellationToken cancellationToken = default)
    {
        return string.IsNullOrEmpty(request.Username) switch
        {
            true =>
                await ExportDownLineAgentWinLossByMasterAgentIdAsync(request)
                    .ConfigureAwait(false),
            false when request is { DownLine: false, MemberType: AccountType.Agent } =>
                await ExportAgentWinLossAsync(request, cancellationToken)
                    .ConfigureAwait(false),
            false when request is { DownLine: true, MemberType: AccountType.Agent } =>
                await ExportDownLineAgentWinLossAsync(request, cancellationToken)
                    .ConfigureAwait(false),
            false when request.MemberType == AccountType.Player =>
                await ExportPlayerWinLossAsync(request, cancellationToken)
                    .ConfigureAwait(false),
            _ => new MemoryStream()
        };
    }

    private async Task<Stream> ExportAgentWinLossAsync(
        ExportAgentWinLossQuery request,
        CancellationToken cancellationToken)
    {
        var childAgents =
            agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var agent = await agentClient
            .GetAgentInfoByUsernameAsync(new GetAgentInfoByUsernameRequest(request.BrandId, request.Username),
                cancellationToken).ConfigureAwait(false);

        var subAgent =
            await childAgents.FirstOrDefaultAsync(x => x.AgentId == agent!.Id, cancellationToken)
                .ConfigureAwait(false);

        var agentId = request.AgentId == agent?.Id ? request.AgentId : subAgent?.AgentId;

        return await reportClient.ExportAgentWinLossAsync(new ExportAgentWinLossRequest(request.BrandId,
            agentId ?? Guid.Empty,
            request.From,
            request.To)).ConfigureAwait(false);
    }

    private async Task<Stream>
        ExportDownLineAgentWinLossByMasterAgentIdAsync(
            ExportAgentWinLossQuery request)
        => await reportClient.ExportDownLineAgentWinLossAsync(new ExportDownLineAgentWinLossRequest(request.BrandId,
            request.AgentId,
            request.From,
            request.To)).ConfigureAwait(false);

    private async Task<Stream>
        ExportDownLineAgentWinLossAsync(
            ExportAgentWinLossQuery request,
            CancellationToken cancellationToken)
    {
        var childAgents =
            agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var agent = await agentClient.GetAgentInfoByUsernameAsync(
            new GetAgentInfoByUsernameRequest(request.BrandId, request.Username),
            cancellationToken).ConfigureAwait(false);

        var subAgent =
            await childAgents.FirstOrDefaultAsync(x => x.AgentId == agent?.Id, cancellationToken)
                .ConfigureAwait(false);

        var agentId = request.AgentId == agent?.Id ? request.AgentId : subAgent?.AgentId;

        return await reportClient
            .ExportDownLineAgentWinLossAsync(new ExportDownLineAgentWinLossRequest(request.BrandId,
                agentId ?? Guid.Empty,
                request.From,
                request.To)).ConfigureAwait(false);
    }

    private async Task<Stream> ExportPlayerWinLossAsync(
        ExportAgentWinLossQuery request,
        CancellationToken cancellationToken)
    {
        var childAgents =
            agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var player =
            await playerClient.GetPlayerInfoByUserNameAsync(request.BrandId,
                request.Username,
                cancellationToken).ConfigureAwait(false);

        if (player == null)
            return await reportClient
                .ExportPlayerWinLossAsync(new ExportPlayerWinLossRequest(
                    "notInAgentLine",
                    request.From,
                    request.To)).ConfigureAwait(false);

        var playerProperties =
            await playerClient.GetPlayerPropertiesAsync(player.PlayerCode,
                cancellationToken).ConfigureAwait(false);

        _ = Guid.TryParse(playerProperties["AgentId"], out var agentId);

        var inAgentLine = await childAgents
            .AnyAsync(x => x.AgentId == agentId || x.ParentAgentId == agentId, cancellationToken)
            .ConfigureAwait(false);

        return await reportClient
            .ExportPlayerWinLossAsync(new ExportPlayerWinLossRequest(
                inAgentLine ? player.PlayerCode : "notInAgentLine",
                request.From,
                request.To)).ConfigureAwait(false);
    }
}