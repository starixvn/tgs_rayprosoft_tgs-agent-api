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

namespace TGS.Services.Report.Application.Queries.Agent
{
    internal class
        GetAgentPlayerWinLossQueryHandler : Handler<GetAgentPlayerWinLossRequest, PagedResponse<AgentPlayerWinLossModel>
        >
    {
        private readonly IAgentDividendService _agentDividendService;
        private readonly IPlayerGameAggregateService _service;
        private readonly ITgsAgentService _tgsAgentService;
        private readonly ITgsPlayerService _tgsPlayerService;

        public GetAgentPlayerWinLossQueryHandler(IPlayerGameAggregateService service,
            ITgsAgentService tgsAgentService,
            ITgsPlayerService tgsPlayerService,
            IAgentDividendService agentDividendService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _tgsAgentService = tgsAgentService;
            _tgsPlayerService = tgsPlayerService;
            _agentDividendService = agentDividendService;
        }

        public override async Task<PagedResponse<AgentPlayerWinLossModel>> Handle(
            GetAgentPlayerWinLossRequest request,
            CancellationToken cancellationToken)
        {
            var agent = await _tgsAgentService.GetAgentAsync(x =>
                x.Id == request.AgentId);

            if (agent == null)
            {
                return CreateInvalidResponse<PagedResponse<AgentPlayerWinLossModel>>(Resources.Agent.NotFound);
            }

            var agentDividends = await _agentDividendService.GetAgentDividendQueries()
                .Where(x => x.AgentId == request.AgentId)
                .Where(x => x.CreatedOn >= request.From)
                .Where(x => x.CreatedOn < request.To).ToArrayAsync(cancellationToken);

            var from = agentDividends.Min(x => x.From);
            var to = agentDividends.Max(x => x.To);

            var playerAggregate = await _service
                .GetPlayerGameAggregateQueries(q =>
                    q.Where(x => x.AgentId == request.AgentId)
                        .Where(x => x.ReportDate >= from && x.ReportDate <= to))
                .GroupBy(g => g.PlayerCode,
                    (playerCode, aggregate) => new
                    {
                        playerCode,
                        LiveDealerWinLossAmount =
                            aggregate.Where(x => x.GameType == GameType.LiveDealer).Sum(x => x.WinLoss),
                        SlotWinLossAmount =
                            aggregate.Where(x =>
                                x.GameType == GameType.Fishing || x.GameType == GameType.Arcade ||
                                x.GameType == GameType.Slot || x.GameType == GameType.TableGame).Sum(x => x.WinLoss),
                        LotteryWinLossAmount = aggregate.Where(x => x.GameType == GameType.Lottery).Sum(x => x.WinLoss),
                        CardGameWinLossAmount =
                            aggregate.Where(x => x.GameType == GameType.CardGame).Sum(x => x.WinLoss),
                        SportsWinLossAmount =
                            aggregate.Where(x => x.GameType == GameType.Sportsbook || x.GameType == GameType.ESports)
                                .Sum(x => x.WinLoss),
                        VirtualSportsWinLossAmount =
                            aggregate.Where(x => x.GameType == GameType.VirtualSports).Sum(x => x.WinLoss)
                    }).ToArrayAsync(cancellationToken);

            var players = await _tgsPlayerService
                .GetPlayerQueries(q => q.Where(x => playerAggregate.Select(x => x.playerCode).Contains(x.PlayerCode)))
                .ToArrayAsync(cancellationToken);
            return await players.Join(
                playerAggregate,
                p => p.PlayerCode,
                winLossPlayer => winLossPlayer.playerCode,
                (p, winLossPlayer) => new AgentPlayerWinLossModel
                {
                    Username = p.Username,
                    LiveDealerWinLossAmount = winLossPlayer.LiveDealerWinLossAmount,
                    SlotWinLossAmount = winLossPlayer.SlotWinLossAmount,
                    LotteryWinLossAmount = winLossPlayer.LotteryWinLossAmount,
                    CardGameWinLossAmount = winLossPlayer.CardGameWinLossAmount,
                    SportsWinLossAmount = winLossPlayer.SportsWinLossAmount,
                    VirtualSportsWinLossAmount = winLossPlayer.VirtualSportsWinLossAmount
                }).CreatePagedResponseAsync(players.Length);
        }
    }
}