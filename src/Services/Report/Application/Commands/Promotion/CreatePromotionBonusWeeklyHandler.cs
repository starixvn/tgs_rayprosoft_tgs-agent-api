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
        CreatePromotionBonusWeeklyHandler : Handler<CreatePromotionBonusWeeklyRequest,
            CreatePromotionBonusResponse>
    {
        private static readonly DateTimeOffset _Now = new(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month,
            DateTimeOffset.Now.Day, 0, 0, 0, new(8, 0, 0));

        private static readonly DateTimeOffset _PreviousMonday = _Now.AddDays(-7).GetMonday();
        private static readonly DateTimeOffset _PreviousSunday = _Now.AddDays(-7).GetSunday();
        private readonly IPlayerGameTypeAggregateService _playerGameTypeAggregateService;
        private readonly IPromotionBonusService _promotionBonusService;
        private readonly ITgsPromotionService _promotionService;

        public CreatePromotionBonusWeeklyHandler(IPromotionBonusService promotionBonusService,
            IPlayerGameTypeAggregateService playerGameTypeAggregateService,
            ITgsPromotionService promotionService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _promotionBonusService = promotionBonusService;
            _playerGameTypeAggregateService = playerGameTypeAggregateService;
            _promotionService = promotionService;
        }

        public override async Task<CreatePromotionBonusResponse> Handle(CreatePromotionBonusWeeklyRequest request,
            CancellationToken cancellationToken)
        {
            var promotions = await _promotionService.GetPromotionQueries()
                .Include(x => x.PromotionGameProviders)
                .Include(x => x.PromotionVipLevels)
                .Include(x => x.PromotionPlayers)
                .Where(x => x.SettlementIntervalType == "Weekly")
                .Where(x => x.From >= _PreviousMonday && x.From <= _PreviousSunday ||
                    x.To >= _PreviousMonday && x.To <= _PreviousSunday ||
                    x.From < _PreviousMonday && x.To > _PreviousSunday
                ).ToArrayAsync(cancellationToken);


            var promotionIds = promotions.Select(x => x.Id);

            if (await _promotionBonusService.GetPromotionBonusQueries()
                .AnyAsync(
                    x => promotionIds.Contains(x.PromotionId) && _PreviousMonday <= x.From &&
                        _PreviousSunday >= x.To, cancellationToken))
            {
                return new()
                {
                    Valid = false,
                    Message =
                        $"Promotion bonus report date range {_PreviousMonday} - {_PreviousSunday} is already existed Weekly."
                };
            }

            var gameProviderIds = promotions.SelectMany(x => x.PromotionGameProviders).Select(x => x.GameProviderId)
                .ToArray();

            var gameTypes = promotions.Select(x => x.GameType).ToArray();

            var playerGameTypeRoundAggregate = await _playerGameTypeAggregateService.GetPlayerGameTypeAggregateQueries()
                .Where(x => x.ReportDate >= _PreviousMonday && x.ReportDate <= _PreviousSunday)
                .Where(x => gameProviderIds.Contains(x.GameProviderId))
                .Where(!gameTypes.Contains(null), x => gameTypes.Contains(x.GameType))
                .ToArrayAsync(cancellationToken);

            var promotionBonus = promotions.Select(x =>
            {
                var id = GuidExtensions.NewSequentialGuid();
                var from = x.From > _PreviousMonday ? x.From : _PreviousMonday;
                var to = x.To < _PreviousSunday ? x.To : _PreviousSunday;
                var (players, amountUsed) = GetPromotionBonusPlayers(id, x, playerGameTypeRoundAggregate, from, to);
                x.AmountUsed = amountUsed;

                return new PromotionBonusModel
                {
                    Id = id,
                    BrandId = x.BrandId,
                    PromotionId = x.Id,
                    From = from,
                    To = to,
                    PromotionType = x.PromotionType,
                    TotalAmountLimit = x.TotalAmountLimit,
                    GameType = x.GameType,
                    Percentage = x.Percentage,
                    BonusAmountLimit = x.BonusAmountLimit,
                    CreatedOn = DateTimeOffset.Now,
                    Players = players
                };
            }).ToArray();

            await _promotionService.UpdatePromotionsAsync(promotions);
            await _promotionBonusService.CreatePromotionBonusAsync(promotionBonus);

            return new() {Valid = true, PromotionBonus = promotionBonus.Where(x => x.AutoSentPlayerBonus).ToArray()};
        }

        private static (PromotionBonusPlayerModel[], decimal) GetPromotionBonusPlayers(Guid id,
            Core.DataServices.Tgs.Models.Promotion promotion, IEnumerable<PlayerGameTypeAggregate> aggregates,
            DateTimeOffset from, DateTimeOffset to)
        {
            var amount = promotion.AmountUsed;
            var players = aggregates
                .Where(pa => promotion.PromotionVipLevels.Select(v => v.Level).Contains(pa.VipLevel))
                .Where(promotion.GameType is not null, pa => pa.GameType == promotion.GameType)
                .Where(pa =>
                    promotion.PromotionGameProviders.Select(pg => pg.GameProviderId)
                        .Contains(pa.GameProviderId))
                .Where(pa => pa.ReportDate >= from)
                .Where(pa => pa.ReportDate >= to);

            return promotion.PromotionType switch
            {
                "ReliefMoneyWeekly" => (promotion.PromotionPlayers.Select(p =>
                    {
                        var playerRebateAmount =
                            players.Where(pa => pa.PlayerCode == p.PlayerCode).Sum(s => s.WinLoss) *
                            promotion.Percentage / 100;

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
                    .ToArray(), amount),
                _ => (Array.Empty<PromotionBonusPlayerModel>(), 0)
            };
        }
    }
}