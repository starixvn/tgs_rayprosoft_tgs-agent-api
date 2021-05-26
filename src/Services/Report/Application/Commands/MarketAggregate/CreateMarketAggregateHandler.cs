using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Settings;
using TGS.Services.Promotion;
using TGS.Services.Report.Application.Models.PlayerRebate;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Commands.MarketAggregate
{
    public record CreateMarketAggregateModel(string Date) : IRequest<MessageResponse>;

    public class
        CreateMarketAggregateHandler : Handler<CreateMarketAggregateModel,
            MessageResponse>
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IMarketAggregateService _marketAggregateService;
        private readonly IPlayerAggregateService _playerAggregateService;
        private readonly ISettingsService _settingsService;

        public CreateMarketAggregateHandler(IPlayerAggregateService playerAggregateService,
            IMarketAggregateService marketAggregateService,
            ISettingsService settingsService,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _playerAggregateService = playerAggregateService;
            _marketAggregateService = marketAggregateService;
            _settingsService = settingsService;
        }

        public override async Task<MessageResponse> Handle(CreateMarketAggregateModel request,
            CancellationToken cancellationToken)
        {
            var previousDay = DateTimeOffset.Now.ToOffset(new(8, 0, 0)).AddDays(-1);
            var reportDate = new DateTimeOffset(previousDay.Year, previousDay.Month, previousDay.Day, 0, 0, 0,
                previousDay.Offset);

            if (request.Date.IsNotNullOrEmpty() &&
                DateTimeOffset.TryParse($"{request.Date} 00:00 +08:00", out var result))
            {
                reportDate = new(result.Year, result.Month, result.Day, 0, 0, 0,
                    result.Offset);
            }

            if (await _marketAggregateService.ExistsAsync(x => x.ReportDate == reportDate))
            {
                return CreateInvalidResponse<CreatePlayerRebateResponse>(
                    $"The market aggregate of {reportDate} have generated.");
            }

            var aggregate = await _playerAggregateService.GetPlayerAggregateQueries()
                .Where(x => x.ReportDate == reportDate)
                .GroupBy(x => x.BrandId, (brandId, markets) =>
                    new Core.DataServices.Tgs.Models.MarketAggregate
                    {
                        BrandId = brandId,
                        ReportDate = reportDate,
                        RegisteredCount = markets.Sum(m => m.RegisteredCount),
                        FirstFundInCount = markets.Sum(m => m.FirstFundInCount),
                        DepositAmount = markets.Sum(m => m.DepositAmount),
                        WithdrawalAmount = markets.Sum(m => m.WithdrawalAmount),
                        AdjustmentAmount = markets.Sum(m => m.AdjustmentAmount),
                        BonusAmount = markets.Sum(m => m.BonusAmount),
                        BetAmount = markets.Sum(m => m.BetAmount),
                        Turnover = markets.Sum(m => m.Turnover),
                        WinLossAmount = markets.Sum(m => m.WinLossAmount),
                        NetProfit = markets.Sum(m => m.NetProfit)
                    }).ToArrayAsync(cancellationToken);

            await _marketAggregateService.CreateMarketAggregatesAsync(aggregate);

            var CurrencyNetProfitsJson =
                await _settingsService.GetSettingAsync(x => x.Key == SettingConsts.CurrencyNetProfit);
            var promotionTypeGroups =
                _jsonSerializationProvider.DeserializeFromString<CurrencyNetProfit[]>(CurrencyNetProfitsJson.Value);


            return CreateValidResponse<MessageResponse>();
        }
    }
}