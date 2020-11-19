using System.Threading.Tasks;

namespace TGS.Apis.FileService.DataService
{
    public interface IRepository<T>
    {
        Task InsertAsync(T obj);
        Task UpdateAsync(T obj);
        Task DeleteAsync(string id);
        Task<T> GetByIdAsync(string id);
        Task<T[]> GetListAsync();
        Task<bool> CanConnection();
    }
}