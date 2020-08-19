using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Infrastructures.Redis
{
    public interface IRedisRepository
    {
        IDatabase GetDb();

        Task DeleteKeyAsync(string key);

        void DeleteKey(string key);

        string GetConnectionStatus();
    }
}
