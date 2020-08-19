using System.Threading.Tasks;

namespace TGS.Infrastructures.Redis
{
    public interface IRedisList<T>
    {
        Task<T[]> GetRangeAsync(long start, long count);

        Task<T[]> TakeAsync(long count, long skip = 0);

        Task AddToStartAsync(T item);

        Task AddToStartAsync(T[] items);

        Task AddToEndAsync(T item);

        Task AddToEndAsync(T[] items);

        Task<T> PopFromStartAsync();

        Task<T> PopFromEndAsync();

        Task ShrinkAsync(long maxSize);

        Task<long> CountAsync();

        Task ClearAsync();

        void SetKeyPrefix(string keyPrefix);

        string GetKey();
    }
}
