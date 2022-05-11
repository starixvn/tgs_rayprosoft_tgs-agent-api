using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tgs.CashService.Client;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;
using TGS.Services.Promotion.Models.PromotionBonus;
using TGS.Services.TgsTransaction.Core;

namespace TGS.Services.Promotion.Handlers.Promotion
{
    public class
        CreatePromotionBonusDailyQueryHandler : Handler<CreatePromotionBonusDailyRequest,
            MessageResponse>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IPlayerBucketRepository _playerBucketService;
        private readonly IPromotionBonusRepository _promotionBonusService;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ITgsRepository _repository;
        private readonly ITgsTransactionRepository _transactionRepository;

        public CreatePromotionBonusDailyQueryHandler(
            IPromotionBonusRepository promotionBonusService,
            IPlayerBucketRepository playerBucketService,
            ICashServiceClient cashServiceClient,
            ITgsTransactionRepository transactionRepository,
            ITgsRepository repository,
            IPromotionRepository promotionRepository,
            IHttpContextAccessor accessor) : base(accessor)
        {
            if (accessor is null)
                throw new ArgumentNullException(nameof(accessor));
            _promotionBonusService =
                promotionBonusService ?? throw new ArgumentNullException(nameof(promotionBonusService));
            _playerBucketService = playerBucketService ?? throw new ArgumentNullException(nameof(playerBucketService));
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _promotionRepository = promotionRepository ?? throw new ArgumentNullException(nameof(promotionRepository));
            _transactionRepository =
                transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        public override async Task<MessageResponse> Handle(
            CreatePromotionBonusDailyRequest request,
            CancellationToken cancellationToken)
        {
            var previousDay = DateTimeOffset.Now.ToOffset(new(8, 0, 0)).AddDays(-1);

            var reportDate = new DateTimeOffset(previousDay.Year,
                previousDay.Month,
                previousDay.Day,
                0,
                0,
                0,
                previousDay.Offset);

            if (request.Date.IsNotNullOrEmpty() &&
                DateTimeOffset.TryParse($"{request.Date} 00:00 +08:00", out var result))
            {
                reportDate = new(result.Year,
                    result.Month,
                    result.Day,
                    0,
                    0,
                    0,
                    result.Offset);
            }

            var promotions = await _promotionRepository.GetPromotionQueries()
                .Include(x => x.PromotionGameProviders)
                .Include(x => x.PromotionVipLevels)
                .Include(x => x.PromotionParticipants)
                .Where(x => x.Status == PromotionStatus.Enabled)
                .Where(x => x.SettlementIntervalType == "Daily")
                .Where(x => x.From <= reportDate && x.To >= reportDate).ToArrayAsync(cancellationToken);

            var promotionIds = promotions.Select(x => x.Id);

            if (await _promotionBonusService.GetPromotionBonusQueries()
                    .AnyAsync(
                        x => promotionIds.Contains(x.PromotionId) && reportDate == x.From &&
                            reportDate == x.To,
                        cancellationToken))
            {
                return new()
                {
                    Valid = false,
                    Message = $"Promotion bonus report date {reportDate} is already existed."
                };
            }

            var gameTypes = promotions.Select(x => x.GameType).ToArray();

            var playerGameRoundAggregate = await _repository.PlayerGameAggregates.AsQueryable()
                .Where(x => x.ReportDate == reportDate)
                .Where(!gameTypes.Contains(null), x => gameTypes.Contains(x.GameType))
                .GroupJoin(_repository.BrandGames,
                    aggregate => new
                    {
                        aggregate.BrandId,
                        aggregate.GameId
                    },
                    brandGame => new
                    {
                        brandGame.BrandId,
                        brandGame.GameId
                    },
                    (aggregate, brandGame) => new
                    {
                        aggregate,
                        brandGame
                    })
                .SelectMany(x => x.brandGame.DefaultIfEmpty(),
                    (x, brandGame) => new
                    {
                        x.aggregate,
                        brandGame
                    })
                .Where(x => x.brandGame == null || x.brandGame.Rebate)
                .ToArrayAsync(cancellationToken);

            var promotionBonus = promotions.Select(x =>
            {
                var id = GuidExtensions.NewSequentialGuid();

                var (players, amountUsed) =
                    GetPromotionBonusPlayers(id, x, playerGameRoundAggregate.Select(p => p.aggregate));
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
                    Players = players.Where(p => p.BonusAmount > 0M).ToArray()
                };
            }).ToArray();
            var autoSentPlayerBonuses = promotionBonus.Where(x => x.AutoSentPlayerBonus).ToArray();
            var playerCodes = autoSentPlayerBonuses.SelectMany(p => p.Players).Select(p => p.PlayerCode);

            var players = await _repository.Players.AsQueryable()
                .Where(x => playerCodes.Contains(x.PlayerCode))
                .Select(x => new
                {
                    x.BrandId,
                    x.PlayerCode,
                    x.Wallets,
                    x.DefaultWalletId
                })
                .ToArrayAsync(cancellationToken);


            await _promotionRepository.UpdatePromotionsAsync(promotions).ConfigureAwait(false);
            var bonus = await _promotionBonusService.CreatePromotionBonusAsync(promotionBonus).ConfigureAwait(false);

            var playerBuckets = bonus
                .SelectMany(x =>
                {
                    return x.PromotionPlayers.Join(players,
                            pb => pb.PlayerCode,
                            p => p.PlayerCode,
                            (pb, p) => new
                            {
                                pb,
                                p
                            })
                        .Select(p => new
                        {
                            p.pb.Id,
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

            await _playerBucketService.CreatePlayerBucketsAsync(playerBuckets.Select(x =>
                new CreatePlayerBucketRequest(x.BrandId,
                    x.PlayerCode,
                    x.DefaultWalletId,
                    x.BucketType,
                    x.BucketAmount)).ToArray()).ConfigureAwait(false);

            var transactionPromotions = new List<TransactionPromotion>();

            foreach (var createPlayerBucketRequest in playerBuckets)
            {
                var response = await _cashServiceClient
                    .PromotionAsync(
                        createPlayerBucketRequest.DefaultWalletId,
                        createPlayerBucketRequest.BonusAmount,
                        null,
                        createPlayerBucketRequest.Id.ToString(),
                        null,
                        cancellationToken)
                    .ConfigureAwait(false);

                transactionPromotions.Add(new()
                {
                    TransactionId = response.TransactionId,
                    PromotionId = createPlayerBucketRequest.PromotionId,
                    PromotionBonusId = createPlayerBucketRequest.PromotionBonusId,
                    PromotionType = createPlayerBucketRequest.PromotionType
                });
            }

            await _transactionRepository.CreateTransactionPromotionsAsync(transactionPromotions.ToArray());

            return CreateValidResponse<MessageResponse>();
        }

        private static (PromotionBonusPlayerModel[], decimal) GetPromotionBonusPlayers(
            Guid id,
            Core.DataServices.Tgs.Models.Promotion promotion, IEnumerable<PlayerGameAggregate> aggregates)
        {
            var amount = promotion.AmountUsed;

            var promotionLevels = promotion.PromotionVipLevels.Select(v => v.Level).ToArray();

            var promotionGameProviderIds = promotion.PromotionGameProviders.Select(pg => pg.GameProviderId).ToArray();

            var players = aggregates
                .Where(promotionLevels.Length > 0,
                    pa => pa.VipLevel != null && promotionLevels.Contains(pa.VipLevel.Value))
                .Where(promotion.GameType is not null, pa => pa.GameType == promotion.GameType)
                .Where(promotionGameProviderIds.Length > 0,
                    pa =>
                        promotionGameProviderIds.Contains(pa.GameProviderId)).ToArray();

            return promotion.PromotionType switch
            {
                "TurnoverRebate" => (promotion.PromotionParticipants.Select(p =>
                    {
                        if (amount > promotion.TotalAmountLimit && !promotion.Unlimited)
                        {
                            return new PromotionBonusPlayerModel
                            {
                                PromotionBonusId = id,
                                PlayerCode = p.PlayerCode,
                                BonusAmount = 0M,
                                PromotionType = promotion.PromotionType
                            };
                        }

                        var playerRebateAmount =
                            players.Where(pa => pa.PlayerCode == p.PlayerCode).Sum(s => s.Turnover) *
                            promotion.Percentage / 100;

                        playerRebateAmount = playerRebateAmount > promotion.BonusAmountLimit
                            ? promotion.BonusAmountLimit
                            : playerRebateAmount;

                        amount += playerRebateAmount;

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
                        if (amount > promotion.TotalAmountLimit && !promotion.Unlimited)
                        {
                            return new PromotionBonusPlayerModel
                            {
                                PromotionBonusId = id,
                                PlayerCode = p.PlayerCode,
                                BonusAmount = 0M,
                                PromotionType = promotion.PromotionType
                            };
                        }

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

                        return new PromotionBonusPlayerModel
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