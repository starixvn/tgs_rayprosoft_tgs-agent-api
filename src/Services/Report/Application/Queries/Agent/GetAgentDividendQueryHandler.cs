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
        GetAgentDividendQueryHandler : Handler<GetAgentDividendsRequest, PagedResponse<AgentDividendModel>>
    {
        private readonly IAgentDividendService _service;
        private readonly ITgsAgentService _tgsAgentService;

        public GetAgentDividendQueryHandler(IAgentDividendService service, ITgsAgentService tgsAgentService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _tgsAgentService = tgsAgentService;
        }

        public override async Task<PagedResponse<AgentDividendModel>> Handle(
            GetAgentDividendsRequest request,
            CancellationToken cancellationToken)
        {
            var agentDividends = await _service.GetAgentDividendQueries(q => q.Where(x => x.BrandId == request.BrandId)
                    .Where(x => x.CreatedOn >= request.From)
                    .Where(x => x.CreatedOn <= request.To))
                .GroupBy(x => x.AgentId,
                    (agentId, list) => new
                    {
                        AgentId = agentId,
                        DepositAmount = list.Sum(l => l.DepositAmount),
                        WithdrawalAmount = list.Sum(l => l.WithdrawalAmount),
                        AdjustmentAmount = list.Sum(l => l.AdjustmentAmount),
                        BonusAmount = list.Sum(l => l.BonusAmount),
                        BetAmount = list.Sum(l => l.BetAmount),
                        Turnover = list.Sum(l => l.Turnover),
                        WinLossAmount = list.Sum(l => l.WinLossAmount),
                        NetProfit = list.Sum(l => l.NetProfit),
                        AgentDividendAmount = list.Sum(l => l.AgentDividendAmount)
                    }).ToArrayAsync(cancellationToken);

            var agentIds = agentDividends.Select(x => x.AgentId).ToArray();
            var agents = await _tgsAgentService.GetAgentQueries(q => q.Where(x => agentIds.Contains(x.Id)))
                .ToArrayAsync(cancellationToken);

            var models = agentDividends.Join(agents, ad => ad.AgentId, a => a.Id,
                (ad, a) => new AgentDividendModel
                {
                    Id = ad.AgentId,
                    Username = a.Username,
                    DepositAmount = ad.DepositAmount,
                    WithdrawalAmount = ad.WithdrawalAmount,
                    AdjustmentAmount = ad.WithdrawalAmount,
                    BonusAmount = ad.BonusAmount,
                    BetAmount = ad.BetAmount,
                    Turnover = ad.Turnover,
                    WinLossAmount = ad.WinLossAmount,
                    NetProfit = ad.NetProfit,
                    AgentDividendAmount = ad.AgentDividendAmount
                }).ToArray();

            return await models.CreatePagedResponseAsync(models.Length);
        }
    }
}