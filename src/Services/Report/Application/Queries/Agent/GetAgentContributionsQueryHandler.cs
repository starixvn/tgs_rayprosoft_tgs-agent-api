using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.ReportHandlers.Agent
{
    internal class
        GetAgentContributionsQueryHandler : Handler<GetAgentContributionsRequest, PagedResponse<AgentContributionModel>>
    {
        private readonly IPlayerAggregateService _service;
        private readonly ITgsAgentService _tgsAgentService;

        public GetAgentContributionsQueryHandler(IPlayerAggregateService service, ITgsAgentService tgsAgentService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _tgsAgentService = tgsAgentService;
        }

        public override async Task<PagedResponse<AgentContributionModel>> Handle(
            GetAgentContributionsRequest request,
            CancellationToken cancellationToken)
        {
            var agent = await _tgsAgentService.GetAgentAsync(x =>
                x.BrandId == request.BrandId && x.Username == request.Username);

            if (agent == null)
            {
                return CreateInvalidResponse<PagedResponse<AgentContributionModel>>(Resources.Agent.NotFound);
            }

            var childAgents = await _tgsAgentService.GetAgentQueries(q => q.Where(x => x.ParentAgentId == agent.Id))
                .Include(x => x.Players)
                .ToArrayAsync(cancellationToken);
            var playerCodes = childAgents.SelectMany(c => c.Players.Select(p => p.PlayerCode));

            var players = agent.Players.GroupJoin(_service.GetPlayerAggregateQueries()
                        .Where(x => x.BrandId == request.BrandId)
                        .Where(x => x.AgentId == agent.Id)
                        .Where(x => x.ReportDate >= request.From && x.ReportDate < request.To), p => p.PlayerCode,
                    playerAggregate => playerAggregate.PlayerCode,
                    (player, players) => new {player, players})
                .SelectMany(x => x.players.DefaultIfEmpty(),
                    (x, playerAggregate) => new {x.player, playerAggregate})
                .GroupBy(x => new {x.player.Username, x.player.PlayerCode}, (player, playerAggregate) =>
                    new AgentContributionModel
                    {
                        Username = player.Username,
                        Type = AccountType.Player,
                        DepositAmount = playerAggregate.Sum(p => p.playerAggregate?.DepositAmount ?? 0),
                        WithdrawalAmount = playerAggregate.Sum(p => p.playerAggregate?.WithdrawalAmount ?? 0),
                        AdjustmentAmount = playerAggregate.Sum(p => p.playerAggregate?.AdjustmentAmount ?? 0),
                        BonusAmount = playerAggregate.Sum(p => p.playerAggregate?.BonusAmount ?? 0),
                        BetAmount = playerAggregate.Sum(p => p.playerAggregate?.BetAmount ?? 0),
                        ValidBetAmount = playerAggregate.Sum(p => p.playerAggregate?.Turnover ?? 0),
                        WinLossAmount = playerAggregate.Sum(p => p.playerAggregate?.WinLossAmount ?? 0),
                        NetProfit = playerAggregate.Sum(p => p.playerAggregate?.NetProfit ?? 0)
                    }).ToArray();

            if (!request.IncludeAgents)
            {
                return await players
                    .CreatePagedResponseAsync(players.Length);
            }

            var childAgentPlayers = childAgents.GroupJoin(_service.GetPlayerAggregateQueries()
                        .Where(x => x.BrandId == request.BrandId)
                        .Where(x => playerCodes
                            .Contains(x.PlayerCode))
                        .Where(x => x.AgentId != null)
                        .Where(x => x.ReportDate >= request.From && x.ReportDate <= request.To), a => a.Id,
                    playerAggregate => playerAggregate.AgentId,
                    (agent, players) => new {agent, players})
                .SelectMany(x => x.players.DefaultIfEmpty(),
                    (x, playerAggregate) => new {x.agent, playerAggregate})
                .GroupBy(x => new {x.agent.Username, x.agent.Id}, (agent, playerAggregate) =>
                    new AgentContributionModel
                    {
                        Username = agent.Username,
                        Type = AccountType.Agent,
                        DepositAmount = playerAggregate.Sum(p => p.playerAggregate?.DepositAmount ?? 0),
                        WithdrawalAmount = playerAggregate.Sum(p => p.playerAggregate?.WithdrawalAmount ?? 0),
                        AdjustmentAmount = playerAggregate.Sum(p => p.playerAggregate?.AdjustmentAmount ?? 0),
                        BonusAmount = playerAggregate.Sum(p => p.playerAggregate?.BonusAmount ?? 0),
                        BetAmount = playerAggregate.Sum(p => p.playerAggregate?.BetAmount ?? 0),
                        ValidBetAmount = playerAggregate.Sum(p => p.playerAggregate?.Turnover ?? 0),
                        WinLossAmount = playerAggregate.Sum(p => p.playerAggregate?.WinLossAmount ?? 0),
                        NetProfit = playerAggregate.Sum(p => p.playerAggregate?.NetProfit ?? 0)
                    }).ToArray();


            return await childAgentPlayers.Union(players)
                .CreatePagedResponseAsync(childAgentPlayers.Length + players.Length);
        }
    }
}