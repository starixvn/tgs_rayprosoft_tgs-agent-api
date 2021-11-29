using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.Agent
{
    internal class
        GetAgentDividendTeamDetailQueryHandler : Handler<GetAgentDividendTeamDetailsRequest,
            PagedResponse<AgentDividendTeamDetailModel>>
    {
        private readonly IAgentDividendService _service;
        private readonly ITgsAgentService _tgsAgentService;

        public GetAgentDividendTeamDetailQueryHandler(IAgentDividendService service, ITgsAgentService tgsAgentService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _tgsAgentService = tgsAgentService;
        }

        public override async Task<PagedResponse<AgentDividendTeamDetailModel>> Handle(
            GetAgentDividendTeamDetailsRequest request,
            CancellationToken cancellationToken)
        {
            var mainAgentDividends = await _service.GetAgentDividendQueries(q =>
                    q.Where(x => x.AgentId == request.AgentId).Where(x => x.CreatedOn >= request.From)
                        .Where(x => x.CreatedOn < request.To)).Include(x => x.AgentDividendMultipleLayers)
                .ToArrayAsync(cancellationToken);

            var agentDividendMultipleLayers = await _service.GetAgentDividendQueries(q =>
                    q.Where(x => x.AgentId == request.AgentId).Where(x => x.CreatedOn >= request.From)
                        .Where(x => x.CreatedOn < request.To))
                .SelectMany(x => x.AgentDividendMultipleLayers).ToArrayAsync(cancellationToken);

            var fistLayerChildAgentIds = agentDividendMultipleLayers
                .Where(x => x.Layer == $"{x.AgentId},{x.DividendProviderAgentId}")
                .Select(x => x.DividendProviderAgentId);

            var agentDividends = await _service.GetAgentDividendQueries(q =>
                    q.Where(x => fistLayerChildAgentIds.Contains(x.AgentId)).Where(x => x.CreatedOn >= request.From)
                        .Where(x => x.CreatedOn < request.To))
                .GroupBy(x => new
                    {
                        x.AgentId,
                        x.From,
                        x.To,
                        x.CreatedOn,
                        x.AgentDividendPercentage
                    },
                    (x, list) => new
                    {
                        x.AgentId,
                        x.From,
                        x.To,
                        x.CreatedOn,
                        x.AgentDividendPercentage,
                        LiveDealerWinLoss = list.Sum(l => l.LiveDealerWinLoss),
                        SlotWinLoss =
                            list.Sum(l => l.SlotWinLoss + l.FishingWinLoss + l.ArcadeWinLoss + l.TableGameWinLoss),
                        LotteryWinLoss = list.Sum(l => l.LotteryWinLoss),
                        CardGameWinLoss = list.Sum(l => l.CardGameWinLoss),
                        SportsWinLoss = list.Sum(l => l.SportsWinLoss + l.ESportsWinLoss),
                        VirtualSportsWinLoss = list.Sum(l => l.VirtualSportsWinLoss),
                        WinLossAmount = list.Sum(l => l.WinLossAmount),
                        AgentDividendAmount = list.Sum(l => l.AgentDividendAmount),
                        ExpenditureAmount = list.Sum(l => l.PromotionAmount + l.RebateAmount + l.AdjustmentAmount),
                        BonusAmount = list.Sum(l => l.RebateAmount + l.PromotionAmount)
                    }).ToArrayAsync(cancellationToken);

            var agentIds = agentDividends.Select(x => x.AgentId).ToArray();
            var agents = await _tgsAgentService.GetAgentQueries(q => q.Where(x => agentIds.Contains(x.Id)))
                .ToArrayAsync(cancellationToken);

            var models = agentDividends.Join(agents,
                ad => ad.AgentId,
                a => a.Id,
                (ad, a) => new AgentDividendTeamDetailModel
                {
                    Id = ad.AgentId,
                    Username = a.Username,
                    SettlementDate = ad.CreatedOn,
                    SettlementDateFrom = ad.From,
                    SettlementDateTo = ad.To,
                    LiveDealerWinLoss = ad.LiveDealerWinLoss,
                    SlotWinLoss = ad.SlotWinLoss,
                    LotteryWinLoss = ad.LotteryWinLoss,
                    CardGameWinLoss = ad.CardGameWinLoss,
                    SportsWinLoss = ad.SportsWinLoss,
                    VirtualSportsWinLoss = ad.VirtualSportsWinLoss,
                    WinLossAmount = ad.WinLossAmount,
                    AgentDividendAmount = mainAgentDividends.Where(x => x.From == ad.From && x.To == ad.To)
                        .SelectMany(x => x.AgentDividendMultipleLayers)
                        .Where(x => x.AgentId != x.DividendProviderAgentId).Sum(x => x.Dividend),
                    AgentDividendPercentage = mainAgentDividends.First(m => m.From == ad.From && m.To == ad.To)
                            .AgentDividendPercentage -
                        ad.AgentDividendPercentage,
                    ExpenditureAmount = ad.ExpenditureAmount,
                    BonusAmount = ad.BonusAmount
                }).ToArray();

            return await models.CreatePagedResponseAsync(models.Length);
        }
    }
}