using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    public interface ITgsQueries<T>
    {
        IQueryable<T> GetQueries();
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> preQuery = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }

    public class TgsQueries<T> : ITgsQueries<T> where T : class
    {
        private readonly IBackendOfficeRepository _repository;

        public TgsQueries(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task<bool> ITgsQueries<T>.ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return _repository.Set<T>().AnyAsync(predicate);
        }

        Task<T> ITgsQueries<T>.GetSingleAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> preQuery)
        {
            preQuery ??= c => c;
            return preQuery(_repository.Set<T>()).FirstOrDefaultAsync(predicate);
        }

        IQueryable<T> ITgsQueries<T>.GetQueries()
        {
            return _repository.Set<T>();
        }
    }
}
