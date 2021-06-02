using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;
using TGS.Services.Promotion;
using TGS.Services.Promotion.Models.PromotionBonus;
using TGS.Services.Report.Infrastructure.DataService;
using TGS.Services.TgsTransaction.Core;
using TGS.Services.WalletService;

namespace TGS.Services.Report.Application.Commands.Promotion
{
    public class
        CreatePromotionBonusMonthlyHandler : Handler<CreatePromotionBonusMonthlyRequest,
            MessageResponse>
    {
        private static readonly DateTimeOffset _CurrentFirstDayOfMonth = _Now.GetFirstDayOfMonth();

        private static readonly DateTimeOffset _Now = new(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month,
            DateTimeOffset.Now.Day, 0, 0, 0, new(8, 0, 0));

        private static readonly DateTimeOffset _PreviousFirstDayOfMonth = _Now.AddMonths(-1).GetFirstDayOfMonth();
        private static readonly DateTimeOffset _PreviousLastDayOfMonth = _Now.AddMonths(-1).GetLastDayOfMonth();
        private readonly IPlayerBucketRepository _playerBucketService;
        private readonly IPlayerGameAggregateService _playerGameTypeAggregateService;
        private readonly ITgsPlayerService _playerService;
        private readonly IPromotionBonusRepository _promotionBonusService;
        private readonly ITgsPromotionService _promotionService;
        private readonly ITgsTransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;

        public CreatePromotionBonusMonthlyHandler(IPromotionBonusRepository promotionBonusService,
            IPlayerGameAggregateService playerGameTypeAggregateService,
            ITgsPromotionService promotionService,
            ITgsPlayerService playerService,
            IPlayerBucketRepository playerBucketService,
            IWalletRepository walletRepository,
            ITgsTransactionRepository transactionRepository,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _promotionBonusService = promotionBonusService;
            _playerGameTypeAggregateService = playerGameTypeAggregateService;
            _promotionService = promotionService;
            _playerService = playerService;
            _playerBucketService = playerBucketService;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }

        public override async Task<MessageResponse> Handle(CreatePromotionBonusMonthlyRequest request,
            CancellationToken cancellationToken)
        {
            var promotions = await _promotionService.GetPromotionQueries()
                .Where(x => x.Status == SwitchStatus.Enabled)
                .Where(x => x.SettlementIntervalType == "Monthly")
                .Where(x => x.From >= _PreviousFirstDayOfMonth && x.From <= _CurrentFirstDayOfMonth ||
                    x.To >= _PreviousFirstDayOfMonth && x.To <= _CurrentFirstDayOfMonth ||
                    x.From < _PreviousFirstDayOfMonth && x.To > _CurrentFirstDayOfMonth
                ).ToArrayAsync(cancellationToken);

            var promotionIds = promotions.Select(x => x.Id);

            if (await _promotionBonusService.GetPromotionBonusQueries()
                .AnyAsync(
                    x => promotionIds.Contains(x.PromotionId) && _PreviousFirstDayOfMonth <= x.From &&
                        _PreviousLastDayOfMonth >= x.To, cancellationToken))
            {
                return new()
                {
                    Valid = false,
                    Message =
                        $"Promotion bonus report date range {_PreviousFirstDayOfMonth} - {_PreviousLastDayOfMonth} is already existed Monthly."
                };
            }

            var gameProviderIds = promotions.SelectMany(x => x.PromotionGameProviders).Select(x => x.GameProviderId)
                .ToArray();

            var gameTypes = promotions.Select(x => x.GameType).ToArray();

            var playerGameRoundAggregate = await _playerGameTypeAggregateService.GetPlayerGameAggregateQueries()
                .Where(x => x.ReportDate >= _PreviousFirstDayOfMonth && x.ReportDate <= _CurrentFirstDayOfMonth)
                .Where(x => gameProviderIds.Contains(x.GameProviderId))
                .Where(!gameTypes.Contains(null), x => gameTypes.Contains(x.GameType))
                .GroupJoin(_playerGameTypeAggregateService.Repository.BrandGames,
                    aggregate => new {aggregate.BrandId, aggregate.GameId},
                    brandGame => new {brandGame.BrandId, brandGame.GameId},
                    (aggregate, brandGame) => new {aggregate, brandGame})
                .SelectMany(x => x.brandGame.DefaultIfEmpty(),
                    (x, brandGame) => new {x.aggregate, brandGame})
                .Where(x => x.brandGame == null || x.brandGame.Rebate).ToArrayAsync(cancellationToken);

            var promotionBonus = promotions.Select(x =>
            {
                var id = GuidExtensions.NewSequentialGuid();
                var from = x.From > _PreviousFirstDayOfMonth ? x.From : _PreviousFirstDayOfMonth;
                var to = x.To < _CurrentFirstDayOfMonth ? x.To : _PreviousLastDayOfMonth;
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

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _promotionService.UpdatePromotionsAsync(promotions);
            await _promotionBonusService.CreatePromotionBonusAsync(promotionBonus);
            await _playerBucketService.CreatePlayerBucketsAsync(playerBuckets.Select(x =>
                new CreatePlayerBucketRequest(x.BrandId, x.PlayerCode, x.DefaultWalletId, x.BucketType,
                    x.BucketAmount)).ToArray()).ConfigureAwait(false);

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

            scope.Complete();

            return CreateValidResponse<MessageResponse>();
        }

        private static (PromotionBonusPlayerModel[], decimal) GetPromotionBonusPlayers(Guid id,
            Core.DataServices.Tgs.Models.Promotion promotion, IEnumerable<PlayerGameAggregate> aggregates,
            DateTimeOffset from, DateTimeOffset to)
        {
            var amount = promotion.AmountUsed;

            var promotionLevels = promotion.PromotionVipLevels.Select(v => v.Level).ToArray();

            var players = aggregates
                .Where(promotionLevels.Length > 0,
                    pa => pa.VipLevel != null && promotionLevels.Contains(pa.VipLevel.Value))
                .Where(promotion.GameType is not null, pa => pa.GameType == promotion.GameType)
                .Where(pa =>
                    promotion.PromotionGameProviders.Select(pg => pg.GameProviderId)
                        .Contains(pa.GameProviderId))
                .Where(pa => pa.ReportDate >= from)
                .Where(pa => pa.ReportDate >= to);

            return promotion.PromotionType switch
            {
                "ReliefMoneyMonthly" => (promotion.PromotionParticipants.Select(p =>
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