using System.Threading.Tasks;
using TGS.Services.PlayerBucket.Core.Models;

namespace TGS.Services.PlayerBucket.Core
{
    public interface IPlayerBucketRepository
    {
        ValueTask<decimal> CreatePlayerBucketWithDepositAsync(CreatePlayerBucketWithDepositRequest request);
        Task CreatePlayerBucketsAsync(CreatePlayerBucketRequest[] request);
        ValueTask<bool> IsBucketFull(string playerCode);
    }
}