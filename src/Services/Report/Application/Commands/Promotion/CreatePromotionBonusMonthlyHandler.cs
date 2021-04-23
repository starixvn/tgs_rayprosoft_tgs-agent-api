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
        CreatePromotionBonusMonthlyHandler : Handler<CreatePromotionBonusMonthlyRequest,
            CreatePromotionBonusResponse>
    {
        private static readonly DateTimeOffset _Now = new(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month,
            DateTimeOffset.Now.Day, 0, 0, 0, new(8, 0, 0, 0));

        private static readonly DateTimeOffset _PreviousFirstDayOfMonth = _Now.GetFirstDayOfMonth();
        private static readonly DateTimeOffset _CurrentFirstDayOfMonth = _Now.AddMonths(1).GetFirstDayOfMonth();
        private readonly IPlayerGameTypeAggregateService _playerGameTypeAggregateService;
        private readonly IPromotionBonusService _promotionBonusService;
        private readonly ITgsPromotionService _promotionService;

        public CreatePromotionBonusMonthlyHandler(IPromotionBonusService promotionBonusService,
            IPlayerGameTypeAggregateService playerGameTypeAggregateService,
            ITgsPromotionService promotionService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _promotionBonusService = promotionBonusService;
            _playerGameTypeAggregateService = playerGameTypeAggregateService;
            _promotionService = promotionService;
        }

        public override async Task<CreatePromotionBonusResponse> Handle(CreatePromotionBonusMonthlyRequest request,
            CancellationToken cancellationToken)
        {
            var promotions = await _promotionService.GetPromotionQueries()
                .Where(x => x.SettlementIntervalType == "Monthly")
                .Where(x => x.From >= _PreviousFirstDayOfMonth && x.From <= _CurrentFirstDayOfMonth ||
                    x.To >= _PreviousFirstDayOfMonth && x.To <= _CurrentFirstDayOfMonth ||
                    x.From < _PreviousFirstDayOfMonth && x.To > _CurrentFirstDayOfMonth
                ).ToArrayAsync(cancellationToken);

            var gameProviderIds = promotions.SelectMany(x => x.PromotionGameProviders).Select(x => x.GameProviderId)
                .ToArray();

            var gameTypes = promotions.Select(x => x.GameType).ToArray();

            var playerGameTypeRoundAggregate = await _playerGameTypeAggregateService.GetPlayerGameTypeAggregateQueries()
                .Where(x => x.ReportDate >= _PreviousFirstDayOfMonth && x.ReportDate <= _CurrentFirstDayOfMonth)
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
            var from = promotion.From > _PreviousFirstDayOfMonth ? promotion.From : (DateTimeOffset?)null;
            var to = promotion.To < _CurrentFirstDayOfMonth ? promotion.To : (DateTimeOffset?)null;
            var players = aggregates
                .Where(pa => promotion.PromotionVipLevels.Select(v => v.Level).Contains(pa.VipLevel))
                .Where(promotion.GameType is not null, pa => pa.GameType == promotion.GameType)
                .Where(pa =>
                    promotion.PromotionGameProviders.Select(pg => pg.GameProviderId)
                        .Contains(pa.GameProviderId))
                .Where(from != null, pa => pa.ReportDate >= from)
                .Where(to != null, pa => pa.ReportDate >= to);

            return promotion.PromotionType switch
            {
                "ReliefMoneyMonthly" => promotion.PromotionPlayers.Select(p =>
                    {
                        var playerRebateAmount =
                            players.Where(pa => pa.PlayerCode == p.PlayerCode).Sum(s => s.WinLoss) *
                            promotion.Percentage;

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