using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    public interface ITgsCommands<in TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity obj);
        Task UpdateAsync(TEntity obj);
        Task DeleteAsync(TEntity obj);
        void BatchInsert(TEntity[] obj);
    }

    public class TgsCommands<TEntity> : ITgsCommands<TEntity> where TEntity : class
    {
        private readonly ITgsRepository _repository;

        public TgsCommands(ITgsRepository repository)
        {
            _repository = repository;
        }

        void ITgsCommands<TEntity>.BatchInsert(TEntity[] obj)
        {
            _repository.BulkInsert(obj);
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
