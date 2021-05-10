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
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.PlayerBucket.Core;
using TGS.Services.PlayerBucket.Core.Models;
using TGS.Services.Promotion.Core;
using TGS.Services.Promotion.Core.Models.PromotionBonus;
using TGS.Services.Report.Infrastructure.DataService;
using TGS.Services.TgsTransaction.Core;
using TGS.Services.WalletService;

namespace TGS.Services.Report.Application.Commands.Promotion
{
    public class
        CreatePromotionBonusDailyQueryHandler : Handler<CreatePromotionBonusDailyRequest,
            MessageResponse>
    {
        private readonly IPlayerBucketRepository _playerBucketService;
        private readonly IPlayerGameTypeAggregateService _playerGameTypeAggregateService;
        private readonly ITgsPlayerService _playerService;
        private readonly IPromotionBonusRepository _promotionBonusService;
        private readonly ITgsPromotionService _promotionService;
        private readonly ITgsTransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;

        public CreatePromotionBonusDailyQueryHandler(IPromotionBonusRepository promotionBonusService,
            IPlayerGameTypeAggregateService playerGameTypeAggregateService,
            ITgsPlayerService playerService,
            ITgsPromotionService promotionService,
            IPlayerBucketRepository playerBucketService,
            IWalletRepository walletRepository,
            ITgsTransactionRepository transactionRepository,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _promotionBonusService = promotionBonusService;
            _playerGameTypeAggregateService = playerGameTypeAggregateService;
            _promotionService = promotionService;
            _playerBucketService = playerBucketService;
            _walletRepository = walletRepository;
            _playerService = playerService;
            _transactionRepository = transactionRepository;
        }

        public override async Task<MessageResponse> Handle(CreatePromotionBonusDailyRequest request,
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
                .Include(x => x.PromotionGameProviders)
                .Include(x => x.PromotionVipLevels)
                .Include(x => x.PromotionParticipants)
                .Where(x => x.Status == SwitchStatus.Enabled)
                .Where(x => x.SettlementIntervalType == "Daily")
                .Where(x => x.From <= reportDate && x.To >= reportDate).ToArrayAsync(cancellationToken);

            var promotionIds = promotions.Select(x => x.Id);

            if (await _promotionBonusService.GetPromotionBonusQueries()
                .AnyAsync(
                    x => promotionIds.Contains(x.PromotionId) && reportDate == x.From &&
                        reportDate == x.To, cancellationToken))
            {
                return new() {Valid = false, Message = $"Promotion bonus report date {reportDate} is already existed."};
            }

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
                var (players, amountUsed) = GetPromotionBonusPlayers(id, x, playerGameTypeRoundAggregate);
                x.AmountUsed = amountUsed;

                return new PromotionBonusModel
                {
                    Id = id,
                    BrandId = x.BrandId,
                    PromotionId = x.Id,
                    From = reportDate,
                    To = reportDate,
                    AutoSentPlayerBonus = x.AutoSentPlayerBonus,
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
                            BucketType = BucketType.Promotion,
                            x.PromotionId,
                            PromotionBonusId = x.Id,
                            x.PromotionType
                        });
                }).ToArray();

            await using var promotionTransaction =
                await _promotionService.TgsRepository.BeginTransactionAsync(IsolationLevel.ReadCommitted)
                    .ConfigureAwait(false);

            await _promotionService.UpdatePromotionsAsync(promotions).ConfigureAwait(false);
            await _promotionBonusService.CreatePromotionBonusAsync(promotionBonus).ConfigureAwait(false);
            await _playerBucketService.CreatePlayerBucketsAsync(playerBuckets.Select(x =>
                new CreatePlayerBucketRequest(x.BrandId, x.PlayerCode, x.DefaultWalletId, x.BucketType,
                    x.BucketAmount)).ToArray()).ConfigureAwait(false);

            await promotionTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            var transactionPromotions = new List<TransactionPromotion>();
            foreach (var createPlayerBucketRequest in playerBuckets)
            {
                var (transactionId, balance) = await _walletRepository.ExchangeAsync(createPlayerBucketRequest.BrandId,
                    createPlayerBucketRequest.DefaultWalletId, createPlayerBucketRequest.PlayerCode,
                    TransactionTypes.Promotion, createPlayerBucketRequest.BonusAmount);

                transactionPromotions.Add(new()
                {
                    TransactionId = transactionId,
                    PromotionId = createPlayerBucketRequest.PromotionId,
                    PromotionBonusId = createPlayerBucketRequest.PromotionBonusId,
                    PromotionType = createPlayerBucketRequest.PromotionType
                });
            }

            await _transactionRepository.CreateTransactionPromotionsAsync(transactionPromotions.ToArray());

            return CreateValidResponse<MessageResponse>();
        }

        private static (PromotionBonusPlayerModel[], decimal) GetPromotionBonusPlayers(Guid id,
            Core.DataServices.Tgs.Models.Promotion promotion, IEnumerable<PlayerGameTypeAggregate> aggregates)
        {
            var amount = promotion.AmountUsed;

            var promotionLevels = promotion.PromotionVipLevels.Select(v => v.Level).ToArray();

            var players = aggregates
                .Where(pa => pa.VipLevel != null && promotionLevels.Contains(pa.VipLevel.Value))
                .Where(promotion.GameType is not null, pa => pa.GameType == promotion.GameType)
                .Where(pa =>
                    promotion.PromotionGameProviders.Select(pg => pg.GameProviderId)
                        .Contains(pa.GameProviderId)).ToArray();

            return promotion.PromotionType switch
            {
                "TurnoverRebate" => (promotion.PromotionParticipants.Select(p =>
                    {
                        var playerRebateAmount =
                            players.Where(pa => pa.PlayerCode == p.PlayerCode).Sum(s => s.Turnover) *
                            promotion.Percentage / 100;

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
                    .ToArray(), amount),
                "ReliefMoneyDaily" => (promotion.PromotionParticipants.Select(p =>
                    {
                        var playerRebateAmount = players.Where(pa => pa.PlayerCode == p.PlayerCode)
                            .Sum(s => s.WinLoss) * promotion.Percentage / 100;

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