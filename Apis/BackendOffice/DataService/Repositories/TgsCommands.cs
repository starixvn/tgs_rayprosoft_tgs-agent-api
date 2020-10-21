using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    public class TgsCommands<TEntity> : ITgsCommands<TEntity> where TEntity : class
    {
        private readonly ITgsRepository _repository;

        public TgsCommands(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task ITgsCommands<TEntity>.BatchInsertAsync(TEntity[] obj)
        {
            return _repository.BulkInsertAsync(obj);
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
