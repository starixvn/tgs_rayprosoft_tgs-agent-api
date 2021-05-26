using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
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

        async Task IPlayerBucketRepository.CreatePlayerBucketsAsync(CreatePlayerBucketRequest[] requests)
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

        async Task IPlayerBucketRepository.FillInBucketAsync(FillInPlayerBucketsRequest request)
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

        async ValueTask<decimal> IPlayerBucketRepository.CreatePlayerBucketWithDepositAsync(
            CreatePlayerBucketWithDepositRequest request)
        {
            var bucketAmount = request.DepositAmount;

            var bucket = new TGS.Core.DataServices.Tgs.Models.PlayerBucket
            {
                PlayerCode = request.PlayerCode,
                BucketAmount = request.DepositAmount,
                BucketType = BucketType.Deposit,
                IsFull = false,
                CreatedOn = DateTimeOffset.Now
            };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _repository.PlayerBuckets
                .AddAsync(bucket).ConfigureAwait(false);

            await _repository.SaveChangesAsync().ConfigureAwait(false);

            await _walletRepository.ExchangeAsync(request.BrandId, request.DefaultWalletId,
                request.PlayerCode, TransactionTypes.Deposit, request.DepositAmount);

            if (request.PromotionId != null)
            {
                var promotion = await _repository.Promotions.FindAsync(request.PromotionId.Value);

                var bonusAmount = promotion.Percentage * request.DepositAmount / 100M;
                bonusAmount = bonusAmount > promotion.BonusAmountLimit
                    ? promotion.BonusAmountLimit
                    : bonusAmount;

                bucketAmount += bonusAmount;

                var promotionBucket = new TGS.Core.DataServices.Tgs.Models.PlayerBucket
                {
                    PlayerCode = request.PlayerCode,
                    BucketAmount = bonusAmount,
                    BucketType = BucketType.Promotion,
                    IsFull = false,
                    CreatedOn = DateTimeOffset.Now
                };

                await _repository.PlayerBuckets.AddAsync(promotionBucket).ConfigureAwait(false);

                await _repository.SaveChangesAsync().ConfigureAwait(false);

                await _walletRepository.ExchangeAsync(request.BrandId, request.DefaultWalletId,
                    request.PlayerCode, TransactionTypes.Promotion, bonusAmount);
            }

            scope.Complete();

            return bucketAmount;
        }

        async ValueTask<bool> IPlayerBucketRepository.IsBucketFullAsync(string playerCode) =>
            await _repository.PlayerBuckets.AllAsync(x => x.PlayerCode == playerCode && x.IsFull);
    }
}