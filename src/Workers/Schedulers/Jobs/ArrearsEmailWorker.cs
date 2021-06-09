using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Mail;
using TGS.Core.Models;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Settings;

namespace TGS.Workers.Schedulers.Jobs
{
    [Queue(Consts.Queue.Report)]
    public sealed class ArrearsEmailWorker : BaseWork<TgsResponse>
    {
        private readonly IEmailClient _emailClient;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ITgsRepository _repository;

        public ArrearsEmailWorker(
            ITgsRepository repository,
            IEmailClient emailClient,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _repository = repository;
            _jsonSerializationProvider = jsonSerializationProvider;
            _emailClient = emailClient;
        }

        public override async Task<TgsResponse> ExecuteAsync(object context)
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
                    (market, brand) => new {market, brand})
                .Select(x => new
                {
                    x.market.BrandId,
                    x.brand.Name,
                    x.brand.Currencies.First().CurrencyCode,
                    x.brand.OperationalEmail,
                    x.market.NetProfit
                })
                .ToArrayAsync();

            var brands = brandNetProfits.Join(currencyNetProfits, bn => bn.CurrencyCode, cn => cn.CurrencyCode,
                    (bn, cn) => new {bn, cn})
                .Select(x =>
                {
                    var netProfitList = x.cn.SendPercentages.Select(percentage =>
                            new
                            {
                                Percentage = percentage,
                                x.cn.NetProfit,
                                calculatedNetProfit = percentage * x.cn.NetProfit / 100M
                            })
                        .OrderBy(o => o.calculatedNetProfit).ToArray();

                    decimal? percentageKey = null;
                    foreach (var netProfit in netProfitList)
                    {
                        if (x.bn.NetProfit < netProfit.calculatedNetProfit)
                        {
                            percentageKey = netProfit.Percentage;
                            break;
                        }
                    }

                    return new
                    {
                        x.bn.BrandId,
                        x.bn.Name,
                        x.bn.OperationalEmail,
                        percentageKey,
                        x.bn.NetProfit
                    };
                }).Where(x => x.percentageKey != null).ToArray();

            var netProfitEmailsJson =
                await _repository.Settings.AsQueryable()
                    .FirstOrDefaultAsync(x => x.Key == SettingConsts.ArrearsEmail);
            var arrearsEmail =
                _jsonSerializationProvider.DeserializeFromString<ArrearsEmail>(netProfitEmailsJson.Value);

            var sendBrandList = brands.Join(arrearsEmail.Daily, b => b.percentageKey, n => n.SendPercentage,
                    (brand, email) => new {brand, email})
                .Select(x => new
                {
                    x.brand.BrandId,
                    x.brand.Name,
                    x.brand.OperationalEmail,
                    Percentage = x.brand.percentageKey,
                    x.brand.NetProfit,
                    x.email.Subject,
                    x.email.Content
                }).ToArray();

            var now = DateTimeOffset.Now;
            foreach (var brand in sendBrandList)
            {
                if (now.Day == 1)
                {
                    var mailMessage = new SendEmailRequest(
                        new[] {brand.OperationalEmail},
                        arrearsEmail.SettlementDay.Subject,
                        string.Format(arrearsEmail.SettlementDay.Content, brand.Name, brand.NetProfit));

                    await _emailClient.SendAsync(mailMessage);
                }
                else
                {
                    var mailMessage = new SendEmailRequest(
                        new[] {brand.OperationalEmail},
                        brand.Subject,
                        string.Format(brand.Content, brand.Name, brand.NetProfit));

                    await _emailClient.SendAsync(mailMessage);
                }
            }

            if (sendBrandList.Any())
            {
                var brandNetProfitStrings = sendBrandList.Select(x => $"{x.Name}: {x.NetProfit}<br>").ToArray();
                var managerMailMessage = new SendEmailRequest(
                    new[] {arrearsEmail.Manager.Emails.JoinAsStringByComma()},
                    arrearsEmail.Manager.Subject,
                    string.Format(arrearsEmail.Manager.Content, brandNetProfitStrings.JoinAsString("")));

                await _emailClient.SendAsync(managerMailMessage);
            }

            return new();
        }
    }
}