using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGS.Infrastructures.Redis
{
    public interface IRedisSortedSet<T>
    {
        void SetKeyPrefix(string keyPrefix);

        string GetKey();

        Task<T> GetWithMinScoreAsync();

        Task<T> GetWithMaxScoreAsync();

        Task TrimLowerAsync(int targetLength);

        Task TrimUpperAsync(int targetLength);

        Task<int> CountAsync();

        Task AddAsync(double score, T item);

        Task AddRangeAsync(IEnumerable<ScoredItem<T>> items);

        Task<T[]> TakeAsync(int count);

        Task<T[]> TakeDescendingAsync(int count);
    }
}
