using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using Tgs.Payment.Clients;
using TGS.Services.Report.Application.Models.LicenseeMarket;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.LicenseeMarket
{
    internal class GetLicenseeMarketsQueryHandler : Handler<GetLicenseeMarketsRequest, LicenseeMarketsResponse>
    {
        private readonly ITgsBrandService _brandService;
        private readonly IPlayerAggregateService _service;

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

            var from = new DateTimeOffset(
                requestFrom.Year,
                requestFrom.Month,
                requestFrom.Day,
                0,
                0,
                0,
                requestFrom.Offset);

            var to = new DateTimeOffset(
                requestTo.Year,
                requestTo.Month,
                requestTo.Day,
                0,
                0,
                0,
                requestTo.Offset);

            var brands = await _brandService.GetBrandQueries(q =>
                        q.Where(x => x.LicenseeId == request.LicenseeId)
                            .Where(!string.IsNullOrEmpty(request.CurrencyCode),
                                x => x.Currencies.Where(x => x.CurrencyCode == request.CurrencyCode).Count() > 0)
                            .Where(request.CurrencyCodes != null,
                                x => request.CurrencyCodes.Contains(x.Currencies.FirstOrDefault().CurrencyCode)),
                    true)
                .Select(x => new
                {
                    x.LicenseeId,
                    BrandId = x.Id,
                    BrandCode = x.Code,
                    CurrencyCode = string.Join(',', x.Currencies.Select(x => x.CurrencyCode).ToArray())
                })
                .ToArrayAsync()
                .ConfigureAwait(false);

            var withdrawals = await _service.Repository.WithdrawForms.AsQueryable()
                .Where(x => brands.Select(x => x.BrandId).Contains(x.BrandId))
                .Where(x => x.UserType == 1 && x.Status == (int)PaymentFlowState.Paid)
                .Where(x => x.CreatedOn >= from && x.CreatedOn < to)
                .GroupBy(x => x.BrandId)
                .Select(withdrawal => new
                {
                    BrandId = withdrawal.Key,
                    WithdrawalAmount = -withdrawal.Sum(w => w.ActualAmount ?? w.Amount)
                })
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            var playerAggregates = await _service.GetPlayerAggregateQueries()
                .Where(x => brands.Select(x => x.BrandId).Contains(x.BrandId))
                .Where(x => x.ReportDate >= from && x.ReportDate < to)
                .GroupBy(x => x.BrandId)
                .Select(playerAggregate => new
                {
                    BrandId = playerAggregate.Key,
                    RegisteredCount = playerAggregate.Sum(m => m.RegisteredCount),
                    FirstFundInCount = playerAggregate.Sum(m => m.FirstFundInCount),
                    DepositAmount = playerAggregate.Sum(m => m.DepositAmount),
                    WithdrawalAmount = playerAggregate.Sum(m => m.WithdrawalAmount),
                    AdjustmentAmount = playerAggregate.Sum(m => m.AdjustmentAmount),
                    BonusAmount = playerAggregate.Sum(m => m.RebateAmount + m.PromotionAmount),
                    BetAmount = playerAggregate.Sum(m => m.BetAmount),
                    Turnover = playerAggregate.Sum(m => m.Turnover),
                    WinLossAmount = playerAggregate.Sum(m => m.WinLossAmount),
                    NetProfit = playerAggregate.Sum(m
                        => m.WinLossAmount - m.AdjustmentAmount - m.RebateAmount - m.PromotionAmount)
                })
                .ToArrayAsync()
                .ConfigureAwait(false);

            var groupDatas = brands
                .GroupJoin(
                    playerAggregates,
                    brand => brand.BrandId,
                    playerAggregate => playerAggregate.BrandId,
                    (brand, playerAggregate) => new
                    {
                        brand,
                        playerAggregate
                    })
                .SelectMany(
                    x => x.playerAggregate.DefaultIfEmpty(),
                    (x, market) => new
                    {
                        x.brand.LicenseeId,
                        x.brand.BrandId,
                        x.brand.BrandCode,
                        x.brand.CurrencyCode,
                        market
                    })
                .GroupBy(
                    x => new
                    {
                        x.LicenseeId,
                        x.BrandId,
                        x.BrandCode,
                        x.CurrencyCode
                    },
                    (market, playerAggregates) =>
                        new LicenseeMarketDataModel
                        {
                            BarndId = market.BrandId,
                            BrandCode = market.BrandCode,
                            CurrencyCode = market.CurrencyCode,
                            RegisteredCount = playerAggregates.Sum(m => m.market?.RegisteredCount ?? 0),
                            FirstFundInCount = playerAggregates.Sum(m => m.market?.FirstFundInCount ?? 0),
                            DepositAmount = playerAggregates.Sum(m => m.market?.DepositAmount ?? 0),
                            WithdrawalAmount =
                                withdrawals.FirstOrDefault(x => x.BrandId == market.BrandId)?.WithdrawalAmount ?? 0,
                            AdjustmentAmount = playerAggregates.Sum(m => m.market?.AdjustmentAmount ?? 0),
                            BonusAmount = playerAggregates.Sum(m => m.market?.BonusAmount ?? 0),
                            BetAmount = playerAggregates.Sum(m => m.market?.BetAmount ?? 0),
                            Turnover = playerAggregates.Sum(m => m.market?.Turnover ?? 0),
                            WinLossAmount = playerAggregates.Sum(m => m.market?.WinLossAmount ?? 0),
                            NetProfit = playerAggregates.Sum(m => m.market?.NetProfit ?? 0)
                        });

            var total = groupDatas
                .GroupBy(c => c.CurrencyCode)
                .Select(Currency => new LicenseeMarketTotalModel
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
                    NetProfit = Currency.Sum(x => x.NetProfit)
                })
                .ToArray();

            var datas = groupDatas.AsQueryable().CreatePagedResponse(request);

            return new LicenseeMarketsResponse
            {
                Datas = datas,
                Totals = total
            };
        }
    }
}