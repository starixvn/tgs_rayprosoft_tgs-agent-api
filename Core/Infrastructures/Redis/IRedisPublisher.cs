using System.Threading.Tasks;

namespace TGS.Infrastructures.Redis
{
    public interface IRedisPublisher
    {
        Task<long> PublishAsync<T>(T message);
    }
}
