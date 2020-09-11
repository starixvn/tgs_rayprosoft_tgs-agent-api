using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TGS.Apis.BackendOffice.DataService
{
    public interface IGenericCommands<TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity obj);
        Task UpdateAsync(TEntity obj);
        Task DeleteAsync(TEntity obj);
    }
    public class GenericCommands<TEntity> : IGenericCommands<TEntity> where TEntity : class
    {
        private readonly IBackendOfficeRepository _repository;

        public GenericCommands(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task IGenericCommands<TEntity>.CreateAsync(TEntity obj)
        {
            _repository.Entry(obj).State = EntityState.Added;
            return _repository.SaveChangesAsync();
        }

        Task IGenericCommands<TEntity>.DeleteAsync(TEntity obj)
        {
            _repository.Entry(obj).State = EntityState.Deleted;
            return _repository.SaveChangesAsync();
        }

        Task IGenericCommands<TEntity>.UpdateAsync(TEntity obj)
        {
            _repository.Entry(obj).State = EntityState.Modified;
            return _repository.SaveChangesAsync();
        }
    }
}
