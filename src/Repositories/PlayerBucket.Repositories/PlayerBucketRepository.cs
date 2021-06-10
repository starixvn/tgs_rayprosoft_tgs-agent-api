using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;
using TGS.Services.WalletService;

namespace PlayerBucket.Repositories
{
    public class PlayerBucketRepository : IPlayerBucketRepository
    {
        private readonly ITgsRepository _repository;
        private readonly IWalletRepository _walletRepository;

        public PlayerBucketRepository(
            ITgsRepository repository,
            IWalletRepository walletRepository)
        {
            _repository = repository;
            _walletRepository = walletRepository;
        }

        public async Task CreatePlayerBucketsAsync(CreatePlayerBucketRequest[] requests)
        {
            var buckets = requests.Select(x => new TGS.Core.DataServices.Tgs.Models.PlayerBucket
            {
                PlayerCode = x.PlayerCode,
                BucketAmount = x.BucketAmount,
                BucketType = x.BucketType,
                IsFull = false,
                CreatedOn = DateTimeOffset.Now
            }).ToArray();

            await _repository.PlayerBuckets.AddRangeAsync(buckets).ConfigureAwait(false);
            await _repository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task FillInBucketAsync(FillInPlayerBucketsRequest request)
        {
            var playerCodes = request.PlayerTurnovers.Select(x => x.PlayerCode).ToArray();
            var playerBuckets = await _repository.PlayerBuckets
                .Where(x => playerCodes.Contains(x.PlayerCode) && !x.IsFull).ToArrayAsync();
            var playerBucketHistories = new List<PlayerBucketHistory>();

            foreach (var player in request.PlayerTurnovers)
            {
                var turnover = player.Turnover;
                var buckets = playerBuckets.Where(x => x.PlayerCode == player.PlayerCode).ToArray();

                foreach (var playerBucket in buckets)
                {
                    if (turnover <= 0)
                    {
                        break;
                    }

                    var amount = 0.0M;
                    if (turnover - (playerBucket.BucketAmount - playerBucket.CurrentBucketAmount) >= 0.0M)
                    {
                        amount = playerBucket.BucketAmount - playerBucket.CurrentBucketAmount;
                        turnover -= amount;
                        playerBucket.IsFull = true;
                        playerBucket.CurrentBucketAmount = playerBucket.BucketAmount;
                    }
                    else
                    {
                        amount = turnover;
                        playerBucket.CurrentBucketAmount += turnover;
                        turnover = 0;
                    }

                    playerBucketHistories.Add(new()
                    {
                        PlayerCode = player.PlayerCode,
                        Turnover = amount,
                        From = request.From,
                        To = request.To,
                        CreatedOn = DateTimeOffset.Now
                    });
                }
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            _repository.PlayerBuckets.UpdateRange(playerBuckets);
            _repository.PlayerBucketHistories.UpdateRange(playerBucketHistories);
            await _repository.SaveChangesAsync().ConfigureAwait(false);

            scope.Complete();
        }

        public async ValueTask<decimal> CreatePlayerBucketWithDepositAsync(
            CreatePlayerBucketWithDepositRequest request)
        {
            var depositBonusAmount = request.DepositAmount;

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _walletRepository.ExchangeAsync(request.BrandId, request.DefaultWalletId,
                request.PlayerCode, TransactionTypes.Deposit, request.DepositAmount);

            if (request.PromotionId != null)
            {
                var promotion = await _repository.Promotions.FindAsync(request.PromotionId.Value);

                var bonusAmount = promotion.Percentage * request.DepositAmount / 100M;
                bonusAmount = bonusAmount > promotion.BonusAmountLimit
                    ? promotion.BonusAmountLimit
                    : bonusAmount;

                depositBonusAmount += bonusAmount;

                var playerBucket = new TGS.Core.DataServices.Tgs.Models.PlayerBucket
                {
                    PlayerCode = request.PlayerCode,
                    BucketAmount = depositBonusAmount * promotion.TurnoverMultiple,
                    BucketType = BucketType.Promotion,
                    IsFull = false,
                    CreatedOn = DateTimeOffset.Now
                };

                var now = DateTimeOffset.Now.ToOffset(new(8, 0, 0));
                var reportDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0,
                    now.Offset);

                var bonusId = GuidExtensions.NewSequentialGuid();
                var bonus = new PromotionBonus
                {
                    Id = bonusId,
                    BrandId = request.BrandId,
                    PromotionId = promotion.Id,
                    From = reportDate,
                    To = reportDate,
                    PromotionType = promotion.PromotionType,
                    TotalAmountLimit = promotion.TotalAmountLimit,
                    GameType = promotion.GameType,
                    Percentage = promotion.Percentage,
                    BonusAmountLimit = promotion.BonusAmountLimit,
                    TurnoverMultiple = promotion.TurnoverMultiple,
                    CreatedOn = DateTimeOffset.Now,
                    PromotionPlayers = new[]
                    {
                        new PromotionPlayer
                        {
                            PlayerCode = request.PlayerCode,
                            PromotionBonusId = bonusId,
                            BonusAmount = bonusAmount,
                            SentOn = promotion.AutoSentPlayerBonus ? DateTimeOffset.Now : null
                        }
                    }.ToHashSet()
                };

                await _repository.PromotionBonus.AddAsync(bonus).ConfigureAwait(false);

                await _repository.PlayerBuckets.AddAsync(playerBucket).ConfigureAwait(false);

                await _repository.SaveChangesAsync().ConfigureAwait(false);

                var (transactionId, _) = await _walletRepository.ExchangeAsync(request.BrandId,
                    request.DefaultWalletId,
                    request.PlayerCode, TransactionTypes.Promotion, bonusAmount);

                var transactionPromotion = new TransactionPromotion
                {
                    TransactionId = transactionId,
                    PromotionId = promotion.Id,
                    PromotionBonusId = bonusId,
                    PromotionType = promotion.PromotionType
                };

                await _repository.TransactionPromotions.AddAsync(transactionPromotion).ConfigureAwait(false);
                await _repository.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                var bucket = new TGS.Core.DataServices.Tgs.Models.PlayerBucket
                {
                    PlayerCode = request.PlayerCode,
                    BucketAmount = request.DepositAmount,
                    BucketType = BucketType.Deposit,
                    IsFull = false,
                    CreatedOn = DateTimeOffset.Now
                };

                await _repository.PlayerBuckets
                    .AddAsync(bucket).ConfigureAwait(false);

                await _repository.SaveChangesAsync().ConfigureAwait(false);
            }

            scope.Complete();

            return depositBonusAmount;
        }

        public async ValueTask<bool> IsBucketFullAsync(string playerCode) =>
            await _repository.PlayerBuckets.Where(x => x.PlayerCode == playerCode).AllAsync(x => x.IsFull);

        public async ValueTask<decimal> ClearPlayerBucketsAsync(string playerCode)
        {
            await _repository.PlayerBuckets.Where(x => x.PlayerCode == playerCode)
                .ForEachAsync(x =>
                {
                    x.CurrentBucketAmount = x.BucketAmount;
                    x.IsFull = true;
                }).ConfigureAwait(false);
            await _repository.SaveChangesAsync().ConfigureAwait(false);

            return await _repository.PlayerBuckets.Where(x => x.PlayerCode == playerCode && x.IsFull == true).SumAsync(x => x.BucketAmount - x.CurrentBucketAmount);
        }
    }
}