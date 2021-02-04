using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Market;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.ReportHandlers.Market
{
    internal class GetMarketsQueryHandler : Handler<GetMarketsRequest, PagedResponse<MarketModel>>
    {
        private readonly IMarketService _service;

        public GetMarketsQueryHandler(IMarketService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        //TODO: calculate every column amount
        public override Task<PagedResponse<MarketModel>> Handle(
            GetMarketsRequest request,
            CancellationToken cancellationToken)
        {
            return _service.GetMarketsAsync()
                .Where(x => x.BrandId == request.BrandId)
                .Where(x => x.CreatedOn >= request.From && x.CreatedOn <= request.To)
                .GroupBy(x => new {x.Id, x.BrandId, x.CreateOnDateString}, (market, markets) => new MarketModel
                {
                    Id = market.Id,
                    BrandId = market.BrandId,
                    DateString = market.CreateOnDateString,
                    RegisteredCount = markets.Sum(m => m.RegisteredCount),
                    FirstFundInCount = markets.Sum(m => m.FirstFundInCount),
                    DepositAmount = markets.Sum(m => m.DepositAmount),
                    WithdrawalAmount = markets.Sum(m => m.WithdrawalAmount),
                    AdjustmentAmount = markets.Sum(m => m.AdjustmentAmount),
                    BonusAmount = markets.Sum(m => m.BonusAmount),
                    ValidBetAmount = markets.Sum(m => m.ValidBetAmount),
                    WinLossAmount = markets.Sum(m => m.WinLossAmount),
                    NetProfit = markets.Sum(m => m.NetProfit)
                }).CreatePagedResponseAsync(request);
        }
    }
}