using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    public interface IBackendOfficeCommands<TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity obj);
        Task UpdateAsync(TEntity obj);
        Task DeleteAsync(TEntity obj);
    }
    public class BackendOfficeCommands<TEntity> : IBackendOfficeCommands<TEntity> where TEntity : class
    {
        private readonly IBackendOfficeRepository _repository;

        public BackendOfficeCommands(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task IBackendOfficeCommands<TEntity>.CreateAsync(TEntity obj)
        {
            _repository.Entry(obj).State = EntityState.Added;
            return _repository.SaveChangesAsync();
        }

        Task IBackendOfficeCommands<TEntity>.DeleteAsync(TEntity obj)
        {
            _repository.Entry(obj).State = EntityState.Deleted;
            return _repository.SaveChangesAsync();
        }

        Task IBackendOfficeCommands<TEntity>.UpdateAsync(TEntity obj)
        {
            _repository.Entry(obj).State = EntityState.Modified;
            return _repository.SaveChangesAsync();
        }
    }
}
