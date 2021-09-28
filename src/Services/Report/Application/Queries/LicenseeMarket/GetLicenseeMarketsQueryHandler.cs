using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.LicenseeMarket;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.LicenseeMarket
{
    internal class GetLicenseeMarketsQueryHandler : Handler<GetLicenseeMarketsRequest, LicenseeMarketsResponse>
    {
        private readonly IPlayerAggregateService _service;
        private readonly ITgsBrandService _brandService;

        public GetLicenseeMarketsQueryHandler(
            IPlayerAggregateService service,
            ITgsBrandService brandService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
        }

        public override async Task<LicenseeMarketsResponse> Handle(
            GetLicenseeMarketsRequest request,
            CancellationToken cancellationToken)
        {
            var offset = new TimeSpan(8, 0, 0);
            var requestFrom = request.From.ToOffset(offset);
            var requestTo = request.To.ToOffset(offset);
            var currencyCodes = request.CurrencyCodes.Where(x => x is not null).ToArray();
            if (request.CurrencyCodes.Where(x => x is not null).Count() == 0)
            {
                currencyCodes = null;
            }
            DateTimeOffset from = new(requestFrom.Year, requestFrom.Month, requestFrom.Day, 0, 0, 0,
                requestFrom.Offset);
            DateTimeOffset to = new(requestTo.Year, requestTo.Month, requestTo.Day, 0, 0, 0,
                requestTo.Offset);

            var brands = await _brandService.GetBrandQueries(q =>
            q.Where(x => x.LicenseeId == request.LicenseeId)
            .Where(currencyCodes is not null, x => currencyCodes.Contains(x.Currencies.FirstOrDefault().CurrencyCode)), true)
                .ToArrayAsync()
                .ConfigureAwait(false);

            var playerAggregates = await _service.GetPlayerAggregateQueries()
                .Where(x => brands.Select(x => x.Id).Contains(x.BrandId))
                .Where(x => x.ReportDate >= from && x.ReportDate < to).ToArrayAsync()
                .ConfigureAwait(false);

            var groupDatas = brands.GroupJoin(
                playerAggregates,
                brand => brand.Id,
                playerAggregate => playerAggregate.BrandId,
                (brand, playerAggregate) => new
                {
                    brand,
                    playerAggregate
                }).SelectMany(x => x.playerAggregate.DefaultIfEmpty(),
                    (x, market) => new
                    {
                        licenseeId = x.brand.LicenseeId,
                        brandId = x.brand.Id,
                        brandCode = x.brand.Code,
                        currencyCode = x.brand.Currencies.First().CurrencyCode,
                        market
                    })
                .GroupBy(x => x, (playerAggregate, playerAggregates) =>
                      new LicenseeMarketDataModel
                      {
                          BarndId = playerAggregate.brandId,
                          BrandCode = playerAggregate.brandCode,
                          CurrencyCode = playerAggregate.currencyCode,
                          RegisteredCount = playerAggregates.Sum(m => m.market?.RegisteredCount ?? 0),
                          FirstFundInCount = playerAggregates.Sum(m => m.market?.FirstFundInCount ?? 0),
                          DepositAmount = playerAggregates.Sum(m => m.market?.DepositAmount ?? 0),
                          WithdrawalAmount = -playerAggregates.Sum(m => m.market?.WithdrawalAmount ?? 0),
                          AdjustmentAmount = playerAggregates.Sum(m => m.market?.AdjustmentAmount ?? 0),
                          BonusAmount =
                              playerAggregates.Sum(m => m.market == null ? 0 : m.market.RebateAmount + m.market.PromotionAmount),
                          BetAmount = playerAggregates.Sum(m => m.market?.BetAmount ?? 0),
                          Turnover = playerAggregates.Sum(m => m.market?.Turnover ?? 0),
                          WinLossAmount = playerAggregates.Sum(m => m.market?.WinLossAmount ?? 0),
                          NetProfit = playerAggregates.Sum(m
                              => m.market == null
                                  ? 0
                                  : m.market.WinLossAmount - m.market.AdjustmentAmount - m.market.RebateAmount -
                                  m.market.PromotionAmount)
                      }).ToArray();

            var total = groupDatas.
                GroupBy(c => c.CurrencyCode).
                Select((Currency) => new LicenseeMarketTotalModel
                {
                    CurrencyCode = Currency.Key,
                    RegisteredCount = Currency.Sum(x => x.RegisteredCount),
                    FirstFundInCount = Currency.Sum(x => x.FirstFundInCount),
                    DepositAmount = Currency.Sum(x => x.DepositAmount),
                    WithdrawalAmount = Currency.Sum(x => x.WithdrawalAmount),
                    AdjustmentAmount = Currency.Sum(x => x.AdjustmentAmount),
                    BonusAmount = Currency.Sum(x => x.BonusAmount),
                    BetAmount = Currency.Sum(x => x.BetAmount),
                    Turnover = Currency.Sum(x => x.Turnover),
                    WinLossAmount = Currency.Sum(x => x.WinLossAmount),
                    NetProfit = Currency.Sum(x => x.NetProfit),
                }).ToArray();

            var datas = groupDatas.AsQueryable().CreatePagedResponse(request);

            return new LicenseeMarketsResponse
            {
                Datas = datas,
                Totals = total
            };
        }
    }
}
