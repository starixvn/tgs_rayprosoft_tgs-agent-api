using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis
{
    public interface IRedisRepository
    {
        void DeleteKey(string key);

        Task DeleteKeyAsync(string key);

        string GetConnectionStatus();

        IDatabase GetDb();
    }
}
