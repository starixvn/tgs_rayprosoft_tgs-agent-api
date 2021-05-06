using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.Services.PlayerBucket.Core;
using TGS.Services.PlayerBucket.Core.Models;

namespace PlayerBucket.Repositories
{
    public class PlayerBucketService : IPlayerBucketService
    {
        private readonly ITgsRepository _repository;

        public PlayerBucketService(ITgsRepository repository)
        {
            _repository = repository;
        }

        async ValueTask<decimal> IPlayerBucketService.CreateDepositAsync(CreatePlayerBucketWithDepositRequest request)
        {
            if (request.PromotionId == null)
            {
                var bucket = new TGS.Core.DataServices.Tgs.Models.PlayerBucket
                {
                    PlayerCode = request.PlayerCode,
                    BucketAmount = request.DepositAmount,
                    BucketType = BucketType.Deposit,
                    IsFull = false,
                    CreatedOn = DateTimeOffset.Now
                };

                await _repository.PlayerBuckets.AddAsync(bucket).ConfigureAwait(false);
                await _repository.SaveChangesAsync().ConfigureAwait(false);

                return request.DepositAmount;
            }
            else
            {
                var promotion = await _repository.Promotions.FindAsync(request.PromotionId.Value);
                var calculateAmount = (promotion.Percentage + 100M) * request.DepositAmount / 100M;
                var bucketAmount = calculateAmount > promotion.BonusAmountLimit
                    ? promotion.BonusAmountLimit
                    : calculateAmount;

                var bucket = new TGS.Core.DataServices.Tgs.Models.PlayerBucket
                {
                    PlayerCode = request.PlayerCode,
                    BucketAmount = bucketAmount,
                    BucketType = BucketType.Deposit,
                    IsFull = false,
                    CreatedOn = DateTimeOffset.Now
                };

                await _repository.PlayerBuckets.AddAsync(bucket).ConfigureAwait(false);
                await _repository.SaveChangesAsync().ConfigureAwait(false);

                return bucketAmount;
            }
        }

        async Task IPlayerBucketService.CreatePlayerBucketsAsync(CreatePlayerBucketRequest[] requests)
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

        async ValueTask<bool> IPlayerBucketService.IsBucketFull(string playerCode) =>
            await _repository.PlayerBuckets.AllAsync(x => x.PlayerCode == playerCode && x.IsFull);
    }
}