using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Market;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.Market
{
    internal class GetMarketsQueryHandler : Handler<GetMarketsRequest, PagedResponse<MarketModel>>
    {
        private readonly IPlayerAggregateService _service;

        public GetMarketsQueryHandler(IPlayerAggregateService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override async Task<PagedResponse<MarketModel>> Handle(
            GetMarketsRequest request,
            CancellationToken cancellationToken)
        {
            var offset = new TimeSpan(8, 0, 0);
            var requestFrom = request.From.ToOffset(offset);
            var requestTo = request.To.ToOffset(offset);

            DateTimeOffset from = new(requestFrom.Year,
                requestFrom.Month,
                requestFrom.Day,
                0,
                0,
                0,
                requestFrom.Offset);

            DateTimeOffset to = new(requestTo.Year,
                requestTo.Month,
                requestTo.Day,
                0,
                0,
                0,
                requestTo.Offset);

            var dates = Enumerable.Range(0, to.Subtract(from).Days)
                .Select(x => from.AddDays(x))
                .ToArray();

            var withdrawals = await _service.Repository.WithdrawForms.AsQueryable()
                .Where(x => x.UserType == 1 && x.Status == 8000)
                .Where(x => x.CreatedOn >= from && x.CreatedOn < to).ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            var withdrawalByDates = withdrawals.GroupBy(x => x.CreatedOn.ToOffset(offset).Date,
                (date, withdrawals) => new
                {
                    Date = date,
                    WithdrawalAmount = withdrawals.Sum(w => w.ActualAmount)
                });

            return await dates.GroupJoin(_service.GetPlayerAggregateQueries()
                        .Where(x => x.BrandId == request.BrandId)
                        .Where(x => x.ReportDate >= from && x.ReportDate < to),
                    date => date,
                    market => market.ReportDate,
                    (date, markets) => new
                    {
                        date,
                        markets
                    })
                .SelectMany(x => x.markets.DefaultIfEmpty(),
                    (x, market) => new
                    {
                        ReportDate = x.date,
                        market
                    })
                .GroupBy(x => new
                    {
                        x.ReportDate
                    },
                    (market, markets) =>
                        new MarketModel
                        {
                            ReportDate = market.ReportDate,
                            RegisteredCount = markets.Sum(m => m.market?.RegisteredCount ?? 0),
                            FirstFundInCount = markets.Sum(m => m.market?.FirstFundInCount ?? 0),
                            DepositAmount = markets.Sum(m => m.market?.DepositAmount ?? 0),
                            WithdrawalAmount =
                                -withdrawalByDates.FirstOrDefault(w => w.Date == market.ReportDate)?.WithdrawalAmount ??
                                0,
                            AdjustmentAmount = markets.Sum(m => m.market?.AdjustmentAmount ?? 0),
                            BonusAmount =
                                markets.Sum(
                                    m => m.market == null ? 0 : m.market.RebateAmount + m.market.PromotionAmount),
                            BetAmount = markets.Sum(m => m.market?.BetAmount ?? 0),
                            Turnover = markets.Sum(m => m.market?.Turnover ?? 0),
                            WinLossAmount = markets.Sum(m => m.market?.WinLossAmount ?? 0),
                            NetProfit = markets.Sum(m
                                => m.market == null
                                    ? 0
                                    : m.market.WinLossAmount - m.market.AdjustmentAmount - m.market.RebateAmount -
                                    m.market.PromotionAmount)
                        })
                .CreatePagedResponseAsync(dates.Length);
        }
    }
}