using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    public interface ITgsCommands<in TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity obj);
        Task UpdateAsync(TEntity obj);
        Task DeleteAsync(TEntity obj);
    }

    public class TgsCommands<TEntity> : ITgsCommands<TEntity> where TEntity : class
    {
        private readonly IBackendOfficeRepository _repository;

        public TgsCommands(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task ITgsCommands<TEntity>.CreateAsync(TEntity obj)
        {
            _repository.Entry(obj).State = EntityState.Added;
            return _repository.SaveChangesAsync();
        }

        Task ITgsCommands<TEntity>.DeleteAsync(TEntity obj)
        {
            _repository.Entry(obj).State = EntityState.Deleted;
            return _repository.SaveChangesAsync();
        }

        Task ITgsCommands<TEntity>.UpdateAsync(TEntity obj)
        {
            _repository.Entry(obj).State = EntityState.Modified;
            return _repository.SaveChangesAsync();
        }
    }
}
