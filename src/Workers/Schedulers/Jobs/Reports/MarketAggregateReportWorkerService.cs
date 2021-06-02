using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Settings;

namespace TGS.Workers.Schedulers.Jobs.Reports
{
    public interface IMarketAggregateReportWorkerService
    {
        Task<TgsResponse> ExecutionAsync();
    }

    public class MarketAggregateReportWorkerService : IMarketAggregateReportWorkerService
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ITgsRepository _repository;

        public MarketAggregateReportWorkerService(
            ITgsRepository repository,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _repository = repository;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        async Task<TgsResponse> IMarketAggregateReportWorkerService.ExecutionAsync()
        {
            var previousDay = DateTimeOffset.Now.ToOffset(new(8, 0, 0)).AddDays(-1);
            var reportDate = new DateTimeOffset(previousDay.Year, previousDay.Month, previousDay.Day, 0, 0, 0,
                previousDay.Offset);

            var currencyNetProfitsJson =
                await _repository.Settings.AsQueryable()
                    .FirstOrDefaultAsync(x => x.Key == SettingConsts.CurrencyNetProfit);
            var currencyNetProfits =
                _jsonSerializationProvider.DeserializeFromString<CurrencyNetProfit[]>(currencyNetProfitsJson.Value);

            var firstDayOfMonth = new DateTimeOffset(reportDate.Year, reportDate.Month, 1, 0, 0, 0, new(8, 0, 0));

            var brandNetProfits = await _repository.PlayerAggregates.AsQueryable()
                .Where(x => x.ReportDate >= firstDayOfMonth && x.ReportDate <= reportDate)
                .GroupBy(x => x.BrandId,
                    (brandId, markets) => new {BrandId = brandId, NetProfit = markets.Sum(m => m.NetProfit)})
                .Where(x => x.NetProfit < 0)
                .Join(_repository.Brands, market => market.BrandId, brand => brand.Id,
                    (market, brands) => new {market, brands})
                .Select(x => new {x.market.BrandId, x.brands.Currencies.First().CurrencyCode, x.market.NetProfit})
                .ToArrayAsync();

            var brands = brandNetProfits.Join(currencyNetProfits, bn => bn.CurrencyCode, cn => cn.CurrencyCode,
                    (bn, cn) => new {bn, cn})
                .Select(x =>
                {
                    Array.Sort(x.cn.SendPercentages);
                    var netProfitList = x.cn.SendPercentages.Select(percentage =>
                        new {x.cn.NetProfit, calculatedNetProfit = percentage * x.cn.NetProfit}).ToArray();

                    decimal? percentageKey = null;
                    foreach (var netProfit in netProfitList)
                    {
                        if (x.bn.NetProfit < -netProfit.calculatedNetProfit)
                        {
                            percentageKey = netProfit.NetProfit;
                        }
                    }

                    return new {x.bn.BrandId, percentageKey};
                }).ToArray();

            var netProfitEmailsJson =
                await _repository.Settings.AsQueryable()
                    .FirstOrDefaultAsync(x => x.Key == SettingConsts.NetProfitEmail);
            var netProfitEmails =
                _jsonSerializationProvider.DeserializeFromString<NetProfitEmail[]>(netProfitEmailsJson.Value);

            var sendBrandList = brands.Join(netProfitEmails, b => b.percentageKey, n => n.SendPercentage,
                    (brand, email) => new {brand, email})
                .Select(x => new
                {
                    x.brand.BrandId, Percentage = x.brand.percentageKey, x.email.Subject, x.email.Content
                });

            return new();
        }
    }
}