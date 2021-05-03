using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
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

        async Task IPlayerBucketService.CreatePlayerBucket(CreatePlayerBucketRequest request)
        {
            var bucket = new TGS.Core.DataServices.Tgs.Models.PlayerBucket
            {
                PlayerCode = request.PlayerCode,
                BucketAmount = request.BucketAmount,
                BucketType = request.BucketType,
                IsFull = false,
                CreatedOn = DateTimeOffset.Now
            };

            await _repository.PlayerBuckets.AddAsync(bucket).ConfigureAwait(false);
            await _repository.SaveChangesAsync().ConfigureAwait(false);
        }

        async Task IPlayerBucketService.CreatePlayerBuckets(CreatePlayerBucketRequest[] requests)
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

        Task IPlayerBucketService.IsBucketFull(string playerCode) =>
            _repository.PlayerBuckets.AllAsync(x => x.PlayerCode == playerCode && x.IsFull);
    }
}