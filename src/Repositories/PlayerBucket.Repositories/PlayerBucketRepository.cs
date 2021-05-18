using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.Services.PlayerBucket.Core;
using TGS.Services.PlayerBucket.Core.Models;
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

        async ValueTask<bool> IPlayerBucketRepository.IsBucketFull(string playerCode) =>
            await _repository.PlayerBuckets.AllAsync(x => x.PlayerCode == playerCode && x.IsFull);
    }
}
