using System.Threading.Tasks;

namespace TGS.Core.Infrastructures.Redis
{
    public interface IRedisPublisher
    {
        Task<long> PublishAsync<T>(T message);
    }
}
