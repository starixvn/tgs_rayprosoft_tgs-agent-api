using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Report.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Services.Report.Application.Models.Promotion;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Commands.Promotion
{
    public class
        CreatePromotionBonusDailyQueryHandler : Handler<CreatePromotionBonusDailyRequest,
            CreatePromotionBonusResponse>
    {
        private readonly IPlayerGameTypeAggregateService _playerGameTypeAggregateService;
        private readonly IPromotionBonusService _promotionBonusService;
        private readonly ITgsPromotionService _promotionService;

        public CreatePromotionBonusDailyQueryHandler(IPromotionBonusService promotionBonusService,
            IPlayerGameTypeAggregateService playerGameTypeAggregateService,
            ITgsPromotionService promotionService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _promotionBonusService = promotionBonusService;
            _playerGameTypeAggregateService = playerGameTypeAggregateService;
            _promotionService = promotionService;
        }

        public override async Task<CreatePromotionBonusResponse> Handle(CreatePromotionBonusDailyRequest request,
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

            var promotions = await _promotionService.GetPromotionQueries()
                .Where(x => x.SettlementIntervalType == "Daily")
                .Where(x => x.From >= reportDate && x.To <= reportDate).ToArrayAsync(cancellationToken);

            var gameProviderIds = promotions.SelectMany(x => x.PromotionGameProviders).Select(x => x.GameProviderId)
                .ToArray();

            var gameTypes = promotions.Select(x => x.GameType).ToArray();

            var playerGameTypeRoundAggregate = await _playerGameTypeAggregateService.GetPlayerGameTypeAggregateQueries()
                .Where(x => x.ReportDate == reportDate)
                .Where(x => gameProviderIds.Contains(x.GameProviderId))
                .Where(!gameTypes.Contains(null), x => gameTypes.Contains(x.GameType))
                .ToArrayAsync(cancellationToken);

            var promotionBonus = promotions.Select(x =>
            {
                var id = GuidExtensions.NewSequentialGuid();

                return new PromotionBonusModel
                {
                    Id = id,
                    PromotionId = x.Id,
                    From = x.From,
                    To = x.To,
                    AutoSentPlayerBonus = x.AutoSentPlayerBonus,
                    PromotionType = x.PromotionType,
                    TotalAmountLimit = x.TotalAmountLimit,
                    GameType = x.GameType,
                    Percentage = x.Percentage,
                    BonusAmountLimit = x.BonusAmountLimit,
                    CreatedOn = DateTimeOffset.Now,
                    Players = GetPromotionBonusPlayers(id, x, playerGameTypeRoundAggregate)
                };
            }).ToArray();

            await _promotionBonusService.CreatePromotionBonusAsync(promotionBonus);

            return new() {Valid = true, PromotionBonus = promotionBonus.Where(x => x.AutoSentPlayerBonus).ToArray()};
        }

        private static PromotionBonusPlayerModel[] GetPromotionBonusPlayers(Guid id,
            Core.DataServices.Tgs.Models.Promotion promotion, IEnumerable<PlayerGameTypeAggregate> aggregates)
        {
            var amount = promotion.AmountUsed;
            var players = aggregates
                .Where(pa => promotion.PromotionVipLevels.Select(v => v.Level).Contains(pa.VipLevel))
                .Where(promotion.GameType is not null, pa => pa.GameType == promotion.GameType)
                .Where(pa =>
                    promotion.PromotionGameProviders.Select(pg => pg.GameProviderId)
                        .Contains(pa.GameProviderId));

            return promotion.PromotionType switch
            {
                "TurnoverRebate" => promotion.PromotionPlayers.Select(p =>
                    {
                        var playerRebateAmount =
                            players.Where(pa => pa.PlayerCode == p.PlayerCode).Sum(s => s.Turnover) *
                            promotion.Percentage;

                        playerRebateAmount = playerRebateAmount > promotion.BonusAmountLimit
                            ? promotion.BonusAmountLimit
                            : playerRebateAmount;

                        amount += playerRebateAmount;
                        if (promotion.TotalAmountLimit < amount)
                        {
                            amount -= playerRebateAmount;
                            playerRebateAmount = promotion.TotalAmountLimit - amount;
                            amount += playerRebateAmount;
                        }

                        return new PromotionBonusPlayerModel
                        {
                            PromotionBonusId = id,
                            PlayerCode = p.PlayerCode,
                            BonusAmount = playerRebateAmount,
                            PromotionType = promotion.PromotionType
                        };
                    })
                    .ToArray(),
                "ReliefMoneyDaily" => promotion.PromotionPlayers.Select(p =>
                    {
                        var playerRebateAmount = players.Where(pa => pa.PlayerCode == p.PlayerCode)
                            .Sum(s => s.WinLoss) * promotion.Percentage;

                        if (playerRebateAmount <= 0)
                        {
                            return new PromotionBonusPlayerModel
                            {
                                PromotionBonusId = id,
                                PlayerCode = p.PlayerCode,
                                BonusAmount = 0,
                                PromotionType = promotion.PromotionType
                            };
                        }

                        playerRebateAmount = playerRebateAmount > promotion.BonusAmountLimit
                            ? promotion.BonusAmountLimit
                            : playerRebateAmount;

                        amount += playerRebateAmount;
                        if (promotion.TotalAmountLimit < amount)
                        {
                            amount -= playerRebateAmount;
                            playerRebateAmount = promotion.TotalAmountLimit - amount;
                            amount += playerRebateAmount;
                        }

                        return new()
                        {
                            PromotionBonusId = id,
                            PlayerCode = p.PlayerCode,
                            BonusAmount = playerRebateAmount,
                            PromotionType = promotion.PromotionType
                        };
                    })
                    .ToArray(),
                _ => Array.Empty<PromotionBonusPlayerModel>()
            };
        }
    }
}