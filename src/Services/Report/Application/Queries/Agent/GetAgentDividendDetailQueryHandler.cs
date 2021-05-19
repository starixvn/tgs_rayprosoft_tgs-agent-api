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
        GetAgentDividendDetailQueryHandler : Handler<GetAgentDividendDetailsRequest,
            PagedResponse<AgentDividendDetailModel>>
    {
        private readonly IAgentDividendService _service;
        private readonly ITgsAgentService _tgsAgentService;

        public GetAgentDividendDetailQueryHandler(IAgentDividendService service, ITgsAgentService tgsAgentService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _tgsAgentService = tgsAgentService;
        }

        public override async Task<PagedResponse<AgentDividendDetailModel>> Handle(
            GetAgentDividendDetailsRequest request,
            CancellationToken cancellationToken)
        {
            var agentDividends = await _service.GetAgentDividendQueries(q =>
                    q.Where(x => x.AgentId == request.AgentId).Where(x => x.CreatedOn >= request.From)
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
                        SlotWinLoss = list.Sum(l => l.SlotWinLoss),
                        LotteryWinLoss = list.Sum(l => l.LotteryWinLoss),
                        CardGameWinLoss = list.Sum(l => l.CardGameWinLoss),
                        SportsWinLoss = list.Sum(l => l.SportsWinLoss),
                        VirtualSportsWinLoss = list.Sum(l => l.VirtualSportsWinLoss),
                        WinLossAmount = list.Sum(l => l.WinLossAmount),
                        AgentDividendAmount = list.Sum(l => l.AgentDividendAmount)
                    }).ToArrayAsync(cancellationToken);

            var agentIds = agentDividends.Select(x => x.AgentId).ToArray();
            var agents = await _tgsAgentService.GetAgentQueries(q => q.Where(x => agentIds.Contains(x.Id)))
                .ToArrayAsync(cancellationToken);

            var models = agentDividends.Join(agents, ad => ad.AgentId, a => a.Id,
                (ad, a) => new AgentDividendDetailModel
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
                    AgentDividendAmount = ad.AgentDividendAmount,
                    AgentDividendPercentage = ad.AgentDividendPercentage
                }).ToArray();

            return await models.CreatePagedResponseAsync(models.Length);
        }
    }
}