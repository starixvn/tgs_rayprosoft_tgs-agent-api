using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Services.BackOffice.Application.Models.PlayerBucket;
using TGS.Services.BackOffice.Infrastructure.DataService;
using Tgs.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.PlayerBucketQueries
{
    public class
        GetPlayerBucketsQuery : IGetService<GetPlayerBucketsRequest, GetPlayerBucketsResponse>
    {
        private readonly IPlayerBucketService _playerBucketService;

        public GetPlayerBucketsQuery(IPlayerBucketService playerBucketService)
            => _playerBucketService =
                playerBucketService ?? throw new ArgumentNullException(nameof(playerBucketService));

        public async ValueTask<GetPlayerBucketsResponse> GetAsync(GetPlayerBucketsRequest request,
            CancellationToken cancellationToken = default)
        {
            var before14DayDate = DateTimeOffset.UtcNow.AddDays(-14);
            var playerBuckets = await _playerBucketService.GetPlayerBucketQueries()
                .Where(x => x.CreatedOn >= before14DayDate || x.IsFull == false)
                .Where(x => x.PlayerCode == request.PlayerCode).ToArrayAsync(cancellationToken);

            return new GetPlayerBucketsResponse
            {
                Valid = true,
                Items = playerBuckets.Select(x => new PlayerBucketModel
                {
                    Id = x.Id,
                    PlayerCode = x.PlayerCode,
                    BucketType = x.BucketType,
                    BucketAmount = x.BucketAmount,
                    CurrentBucketAmount = x.CurrentBucketAmount,
                    CreatedOn = x.CreatedOn
                }).ToArray()
            };
        }
    }
}