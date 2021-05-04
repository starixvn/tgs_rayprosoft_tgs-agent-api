using System.Threading.Tasks;
using TGS.Services.PlayerBucket.Core.Models;

namespace TGS.Services.PlayerBucket.Core
{
    public interface IPlayerBucketService
    {
        ValueTask<decimal> CreateDepositAsync(CreatePlayerBucketWithDepositRequest request);
        Task CreatePlayerBucketsAsync(CreatePlayerBucketRequest[] request);
        ValueTask<bool> IsBucketFull(string playerCode);
    }
}