using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Report.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.PlayerBucket.Core;
using TGS.Services.PlayerBucket.Core.Models;
using TGS.Services.Promotion.Core;
using TGS.Services.Promotion.Core.Models.PromotionBonus;
using TGS.Services.Report.Infrastructure.DataService;
using TGS.Services.Wallet;

namespace TGS.Services.Report.Application.Commands.Promotion
{
    public class
        CreatePromotionBonusWeeklyHandler : Handler<CreatePromotionBonusWeeklyRequest,
            MessageResponse>
    {
        private static readonly DateTimeOffset _Now = new(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month,
            DateTimeOffset.Now.Day, 0, 0, 0, new(8, 0, 0));

        private static readonly DateTimeOffset _PreviousMonday = _Now.AddDays(-7).GetMonday();
        private static readonly DateTimeOffset _PreviousSunday = _Now.AddDays(-7).GetSunday();
        private readonly IPlayerBucketService _playerBucketService;
        private readonly IPlayerGameTypeAggregateService _playerGameTypeAggregateService;
        private readonly ITgsPlayerService _playerService;
        private readonly IPromotionBonusService _promotionBonusService;
        private readonly ITgsPromotionService _promotionService;
        private readonly IWalletRepository _walletRepository;

        public CreatePromotionBonusWeeklyHandler(IPromotionBonusService promotionBonusService,
            IPlayerGameTypeAggregateService playerGameTypeAggregateService,
            ITgsPromotionService promotionService,
            ITgsPlayerService playerService,
            IPlayerBucketService playerBucketService,
            IWalletRepository walletRepository,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _promotionBonusService = promotionBonusService;
            _playerGameTypeAggregateService = playerGameTypeAggregateService;
            _promotionService = promotionService;
            _playerBucketService = playerBucketService;
            _walletRepository = walletRepository;
            _playerService = playerService;
        }

        public override async Task<MessageResponse> Handle(CreatePromotionBonusWeeklyRequest request,
            CancellationToken cancellationToken)
        {
            var promotions = await _promotionService.GetPromotionQueries()
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
                    TurnoverMultiple = x.TurnoverMultiple,
                    CreatedOn = DateTimeOffset.Now,
                    SentOn = x.AutoSentPlayerBonus ? DateTimeOffset.Now : null,
                    Players = players
                };
            }).ToArray();

            var autoSentPlayerBonuses = promotionBonus.Where(x => x.AutoSentPlayerBonus).ToArray();
            var playerCodes = autoSentPlayerBonuses.SelectMany(p => p.Players).Select(p => p.PlayerCode);

            var players = await _playerService.GetPlayerQueries()
                .Where(x => playerCodes.Contains(x.PlayerCode))
                .Select(x => new {x.BrandId, x.PlayerCode, x.Wallets, x.DefaultWalletId})
                .ToArrayAsync(cancellationToken);

            var playerBuckets = autoSentPlayerBonuses
                .SelectMany(x =>
                {
                    return x.Players.Join(players, pb => pb.PlayerCode, p => p.PlayerCode, (pb, p) => new {pb, p})
                        .Select(p => new
                        {
                            x.BrandId,
                            p.p.PlayerCode,
                            p.p.DefaultWalletId,
                            BucketAmount = p.pb.BonusAmount * x.TurnoverMultiple,
                            p.pb.BonusAmount,
                            BucketType = BucketType.Promotion
                        });
                }).ToArray();

            await using var promotionTransaction =
                await _promotionService.TgsRepository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await _promotionService.UpdatePromotionsAsync(promotions);
            await _promotionBonusService.CreatePromotionBonusAsync(promotionBonus);
            await _playerBucketService.CreatePlayerBucketsAsync(playerBuckets.Select(x =>
                new CreatePlayerBucketRequest(x.BrandId, x.PlayerCode, x.DefaultWalletId, x.BucketType,
                    x.BucketAmount)).ToArray()).ConfigureAwait(false);

            await promotionTransaction.CommitAsync(cancellationToken);

            foreach (var createPlayerBucketRequest in playerBuckets)
            {
                await _walletRepository.ExchangeAsync(createPlayerBucketRequest.BrandId,
                    createPlayerBucketRequest.DefaultWalletId, createPlayerBucketRequest.PlayerCode,
                    TransactionTypes.Promotion, createPlayerBucketRequest.BonusAmount);
            }

            return CreateValidResponse<MessageResponse>();
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
                "ReliefMoneyWeekly" => (promotion.PromotionParticipants.Select(p =>
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