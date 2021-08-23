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
    [Queue(Consts.Queue.ArrearsEmail)]
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
            var now = DateTimeOffset.Now;
            var previousDay = now.ToOffset(new(8, 0, 0)).AddDays(-1);
            var reportDate = new DateTimeOffset(previousDay.Year, previousDay.Month, previousDay.Day, 0, 0, 0,
                previousDay.Offset);

            var firstDayOfMonth = new DateTimeOffset(reportDate.Year, reportDate.Month, 1, 0, 0, 0, new(8, 0, 0));
            var isFirstDay = now.Day == 1;

            var netProfitEmailsJson =
                await _repository.Settings.AsQueryable()
                    .FirstOrDefaultAsync(x => x.Key == SettingConsts.ArrearsEmail);
            var arrearsEmail =
                _jsonSerializationProvider.DeserializeFromString<ArrearsEmail>(netProfitEmailsJson.Value);
            var percentages = arrearsEmail.Daily.Select(x => x.SendPercentage).ToArray();

            var brandNetProfits = await GetBrandNetProfitsAsync(isFirstDay ? reportDate : now, firstDayOfMonth,
                reportDate, percentages);

            var sendBrandList = brandNetProfits.Where(x => x.PercentageKey != null).Join(arrearsEmail.Daily,
                    b => b.PercentageKey, n => n.SendPercentage,
                    (brand, email) => new
                    {
                        brand,
                        email
                    })
                .Select(x => new
                {
                    x.brand.BrandId,
                    x.brand.Name,
                    x.brand.OperationalEmail,
                    Percentage = x.brand.PercentageKey,
                    x.brand.NetProfitAndPay,
                    x.email.Subject,
                    x.email.Content
                }).ToArray();


            foreach (var brand in sendBrandList)
            {
                if (now.Day == 1)
                {
                    var mailMessage = new SendEmailRequest(
                        new[]
                        {
                            brand.OperationalEmail
                        },
                        arrearsEmail.SettlementDay.Subject,
                        string.Format(arrearsEmail.SettlementDay.Content, brand.Name, brand.NetProfitAndPay));

                    await _emailClient.SendAsync(mailMessage);
                }
                else
                {
                    var mailMessage = new SendEmailRequest(
                        new[]
                        {
                            brand.OperationalEmail
                        },
                        brand.Subject,
                        string.Format(brand.Content, brand.Name, brand.NetProfitAndPay));

                    await _emailClient.SendAsync(mailMessage);
                }
            }

            if (sendBrandList.Any())
            {
                var brandNetProfitStrings = sendBrandList.Select(x => $"{x.Name}: {x.NetProfitAndPay} \r\n").ToArray();
                var managerMailMessage = new SendEmailRequest(
                    new[]
                    {
                        arrearsEmail.Manager.Emails.JoinAsStringByComma()
                    },
                    arrearsEmail.Manager.Subject,
                    string.Format(arrearsEmail.Manager.Content, brandNetProfitStrings.JoinAsString("")));

                await _emailClient.SendAsync(managerMailMessage);
            }

            if (!isFirstDay)
            {
                await UpdatePlatformCreditsAsync(brandNetProfits);
            }

            return new();
        }

        private async Task<BrandNetProfitModel[]> GetBrandNetProfitsAsync(DateTimeOffset payTime,
            DateTimeOffset from,
            DateTimeOffset to, decimal[] percentages)
        {
            var brandNetProfits = await _repository.PlayerAggregates.AsQueryable()
                .Where(x => x.ReportDate >= from && x.ReportDate <= to)
                .GroupBy(x => x.BrandId,
                    (brandId, markets) => new
                    {
                        BrandId = brandId,
                        NetProfit = markets.Sum(m
                            => m.WinLossAmount - m.AdjustmentAmount - m.RebateAmount - m.PromotionAmount)
                    })
                .Where(x => x.NetProfit < 0)
                .Join(_repository.Brands, market => market.BrandId, brand => brand.Id,
                    (market, brand) => new
                    {
                        market,
                        brand
                    })
                .Join(_repository.PlatformCredits, group => group.market.BrandId,
                    platformCredit => platformCredit.BrandId, (group, platformCredit) => new
                    {
                        group.market,
                        group.brand,
                        platformCredit
                    })
                .Select(x => new
                {
                    x.market.BrandId,
                    x.brand.Name,
                    x.platformCredit.Credit,
                    x.brand.OperationalEmail,
                    x.market.NetProfit,
                    PayAmount = x.platformCredit.BrandPayRecords
                        .Where(p => p.CreatedOn >= from && p.CreatedOn < payTime).Sum(p => p.PayAmount)
                })
                .ToArrayAsync();

            return brandNetProfits.Select(x =>
            {
                var netProfitAndPay = x.NetProfit - x.PayAmount;
                var netProfitList = percentages.Select(percentage =>
                        new
                        {
                            Percentage = percentage,
                            CalculatedCredit = percentage * x.Credit / 100M
                        })
                    .OrderByDescending(o => o.CalculatedCredit).ToArray();

                decimal? percentageKey = null;
                foreach (var netProfit in netProfitList)
                {
                    if (netProfitAndPay > netProfit.CalculatedCredit)
                    {
                        percentageKey = netProfit.Percentage;
                        break;
                    }
                }

                return new BrandNetProfitModel
                {
                    BrandId = x.BrandId,
                    Name = x.Name,
                    OperationalEmail = x.OperationalEmail,
                    PercentageKey = percentageKey,
                    NetProfitAndPay = netProfitAndPay
                };
            }).ToArray();
        }

        private async Task UpdatePlatformCreditsAsync(BrandNetProfitModel[] models)
        {
            var brandIds = models.Select(x => x.BrandId).ToArray();
            var platformCredits = await _repository.PlatformCredits.AsQueryable().Where(x => brandIds.Contains(x.Id))
                .ToArrayAsync();
            foreach (var platformCredit in platformCredits)
            {
                platformCredit.Balance = platformCredit.Credit -
                    models.First(x => x.BrandId == platformCredit.BrandId).NetProfitAndPay;
            }

            _repository.PlatformCredits.UpdateRange(platformCredits);
        }

        private class BrandNetProfitModel
        {
            public Guid BrandId { get; init; }
            public string Name { get; init; }
            public string OperationalEmail { get; init; }
            public decimal? PercentageKey { get; init; }
            public decimal NetProfitAndPay { get; init; }
        }
    }
}