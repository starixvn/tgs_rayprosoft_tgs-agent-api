using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Service.PlayerBuckets;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class GetPlayerBucketAmountQueryService : IGetService<GetPlayerWithdrawRequireAmountQuery, decimal>
{
    private readonly IPlayerBucketClient _playerBucketClient;

    public GetPlayerBucketAmountQueryService(IPlayerBucketClient playerBucketClient)
    {
        _playerBucketClient = playerBucketClient ?? throw new ArgumentNullException(nameof(playerBucketClient));
    }

    public ValueTask<decimal> GetAsync(GetPlayerWithdrawRequireAmountQuery query, CancellationToken cancellationToken = default)
        => _playerBucketClient.ReadBucketsAsync(query.PlayerCode, cancellationToken)
            .AsTask()
            .ToAsyncEnumerable()
            .Select(result => result.Amount)
            .SingleAsync(cancellationToken: cancellationToken);
}
