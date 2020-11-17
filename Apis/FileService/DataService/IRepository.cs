using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileService.DataService
{
    public interface IRepository<T>
    {
        Task InsertAsync(T obj);
        Task UpdateAsync(T obj);
        Task DeleteAsync(string id);
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetListAsync();
        Task<bool> CanConnection();
    }
}