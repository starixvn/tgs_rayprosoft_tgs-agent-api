using System.Threading.Tasks;
using TGS.Services.PlayerBucket.Core.Models;

namespace TGS.Services.PlayerBucket.Core
{
    public interface IPlayerBucketService
    {
        Task CreatePlayerBucket(CreatePlayerBucketRequest request);
        Task CreatePlayerBuckets(CreatePlayerBucketRequest[] request);
        Task IsBucketFull(string playerCode);
    }
}