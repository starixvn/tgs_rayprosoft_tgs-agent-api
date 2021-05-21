using System.Threading.Tasks;
using TGS.Services.PlayerBucket.Models;

namespace TGS.Services.PlayerBucket
{
    public interface IPlayerBucketRepository
    {
        ValueTask<decimal> CreatePlayerBucketWithDepositAsync(CreatePlayerBucketWithDepositRequest request);
        Task CreatePlayerBucketsAsync(CreatePlayerBucketRequest[] request);
        ValueTask<bool> IsBucketFullAsync(string playerCode);
    }
}
