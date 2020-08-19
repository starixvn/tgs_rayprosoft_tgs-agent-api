using System;
using System.Threading.Tasks;

namespace TGS.Infrastructures.Redis
{
    public interface IRedisSubscriber
    {
        Task SubscribeAsync<T>(Action<T> handler);
        Task UnsubscribeAsync<T>();
    }
}
