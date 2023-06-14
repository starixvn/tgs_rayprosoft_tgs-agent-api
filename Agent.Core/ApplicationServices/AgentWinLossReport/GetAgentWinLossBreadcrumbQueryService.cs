using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using Tgs.ServiceUtility;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.PlayerService.Client;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.AgentWinLoss;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class
    GetAgentWinLossBreadcrumbQueryService : IGetService<GetAgentWinLossBreadcrumbQuery,
        IEnumerable<AgentWinLossBreadcrumb>>
{
    private readonly IAgentClient _agentClient;
    private readonly IPlayerServiceClient _playerServiceClient;
    private readonly IReportClient _reportClient;

    public GetAgentWinLossBreadcrumbQueryService(
        IReportClient reportClient,
        IAgentClient agentClient,
        IPlayerServiceClient playerServiceClient)
    {
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
        _agentClient = agentClient ?? throw new ArgumentNullException(nameof(agentClient));
        _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
    }

    public async ValueTask<IEnumerable<AgentWinLossBreadcrumb>> GetAsync(
        GetAgentWinLossBreadcrumbQuery request,
        CancellationToken cancellationToken = default)
    {
        var childAgents = _agentClient.GetChildAgentInfosAsync(new GetChildAgentInfosRequest(request.AgentId), cancellationToken);

        var agent = !string.IsNullOrEmpty(request.Username) && request.MemberType == AccountType.Agent
            ? await childAgents
                .FirstOrDefaultAsync(x => x.AgentUsername == request.Username, cancellationToken)
            : (ChildAgentInfoModel?)null;

        var player = !string.IsNullOrEmpty(request.Username) && request.MemberType == AccountType.Player
            ? await _playerServiceClient.GetPlayerInfoByUserNameAsync(request.BrandId,
                request.Username,
                cancellationToken)
            : null;

        var reportRequest = new GetBreadcrumbRequest
        {
            AgentId = agent?.AgentId,
            PlayerCode = player?.PlayerCode,
            RoundId = request.RoundId
        };

        var response = await _reportClient.GetAgentWinLossBreadcrumbAsync(reportRequest);

        var haveSubAgents = await childAgents.AnyAsync(
            x => response.Select(r => r.Id).Contains(x.ParentAgentId.ToString()) ||
                response.Select(r => r.Id).Contains(x.AgentId.ToString()), cancellationToken);

        return haveSubAgents
            ? response.Select(x => new AgentWinLossBreadcrumb
            {
                Username = x.Name,
                Id = x.Id,
                MemberType = x.Type
            })
            : Array.Empty<AgentWinLossBreadcrumb>();
    }
}