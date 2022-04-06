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
        CreatePromotionBonusMonthlyHandler : Handler<CreatePromotionBonusMonthlyRequest,
            MessageResponse>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly DateTimeOffset _currentFirstDayOfMonth;

        private readonly DateTimeOffset _now = new(DateTimeOffset.Now.Year,
            DateTimeOffset.Now.Month,
            DateTimeOffset.Now.Day,
            0,
            0,
            0,
            new(8, 0, 0));

        private readonly IPlayerBucketRepository _playerBucketService;

        private readonly DateTimeOffset _previousFirstDayOfMonth;
        private readonly DateTimeOffset _previousLastDayOfMonth;
        private readonly IPromotionBonusRepository _promotionBonusService;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ITgsRepository _repository;
        private readonly ITgsTransactionRepository _transactionRepository;

        public CreatePromotionBonusMonthlyHandler(
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
            _previousLastDayOfMonth = _now.AddMonths(-1).GetLastDayOfMonth();
            _previousFirstDayOfMonth = _now.AddMonths(-1).GetFirstDayOfMonth();
            _currentFirstDayOfMonth = _now.GetFirstDayOfMonth();
        }

        public override async Task<MessageResponse> Handle(
            CreatePromotionBonusMonthlyRequest request,
            CancellationToken cancellationToken)
        {
            var promotions = await _promotionRepository.GetPromotionQueries()
                .Where(x => x.Status == PromotionStatus.Enabled)
                .Where(x => x.SettlementIntervalType == "Monthly")
                .Where(x => x.From >= _previousFirstDayOfMonth && x.From <= _currentFirstDayOfMonth ||
                    x.To >= _previousFirstDayOfMonth && x.To <= _currentFirstDayOfMonth ||
                    x.From < _previousFirstDayOfMonth && x.To > _currentFirstDayOfMonth
                ).ToArrayAsync(cancellationToken);

            var promotionIds = promotions.Select(x => x.Id);

            if (await _promotionBonusService.GetPromotionBonusQueries()
                    .AnyAsync(
                        x => promotionIds.Contains(x.PromotionId) && _previousFirstDayOfMonth <= x.From &&
                            _previousLastDayOfMonth >= x.To,
                        cancellationToken))
            {
                return new()
                {
                    Valid = false,
                    Message =
                        $"Promotion bonus report date range {_previousFirstDayOfMonth} - {_previousLastDayOfMonth} is already existed Monthly."
                };
            }

            var gameTypes = promotions.Select(x => x.GameType).ToArray();

            var playerGameRoundAggregate = await _repository.PlayerGameAggregates.AsQueryable()
                .Where(x => x.ReportDate >= _previousFirstDayOfMonth && x.ReportDate <= _currentFirstDayOfMonth)
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
                .Where(x => x.brandGame == null || x.brandGame.Rebate).ToArrayAsync(cancellationToken);

            var promotionBonus = promotions.Select(x =>
            {
                var id = GuidExtensions.NewSequentialGuid();
                var from = x.From > _previousFirstDayOfMonth ? x.From : _previousFirstDayOfMonth;
                var to = x.To < _currentFirstDayOfMonth ? x.To : _previousLastDayOfMonth;

                var (players, amountUsed) =
                    GetPromotionBonusPlayers(id, x, playerGameRoundAggregate.Select(p => p.aggregate), from, to);
                x.AmountUsed = amountUsed;

                return new PromotionBonusModel
                {
                    Id = id,
                    BrandId = x.BrandId,
                    PromotionId = x.Id,
                    From = from,
                    To = to,
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

            var playerBuckets = autoSentPlayerBonuses
                .SelectMany(x =>
                {
                    return x.Players.Join(players,
                            pb => pb.PlayerCode,
                            p => p.PlayerCode,
                            (pb, p) => new
                            {
                                pb,
                                p
                            })
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

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _promotionRepository.UpdatePromotionsAsync(promotions);
            await _promotionBonusService.CreatePromotionBonusAsync(promotionBonus);

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
                        null,
                        null)
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

            scope.Complete();

            return CreateValidResponse<MessageResponse>();
        }

        private static (PromotionBonusPlayerModel[], decimal) GetPromotionBonusPlayers(
            Guid id,
            Core.DataServices.Tgs.Models.Promotion promotion, IEnumerable<PlayerGameAggregate> aggregates,
            DateTimeOffset from, DateTimeOffset to)
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
                        promotionGameProviderIds.Contains(pa.GameProviderId))
                .Where(pa => pa.ReportDate >= from)
                .Where(pa => pa.ReportDate >= to);

            return promotion.PromotionType switch
            {
                "ReliefMoneyMonthly" => (promotion.PromotionParticipants.Select(p =>
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