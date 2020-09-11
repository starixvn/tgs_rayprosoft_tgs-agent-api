using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TGS.Apis.BackendOffice.DataService.Queries
{
    public interface IGenericQueries<T>
    {
        IQueryable<T> GetQueries();
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> preQuery = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }

    public class GenericQueries<T> : IGenericQueries<T> where T : class
    {
        private readonly IBackendOfficeRepository _repository;

        public GenericQueries(IBackendOfficeRepository repository)
        {
            _repository = repository;
        }

        Task<bool> IGenericQueries<T>.ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return _repository.Set<T>().AnyAsync(predicate);
        }

        Task<T> IGenericQueries<T>.GetSingleAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> preQuery)
        {
            preQuery ??= c => c;
            return preQuery(_repository.Set<T>()).FirstOrDefaultAsync(predicate);
        }

        IQueryable<T> IGenericQueries<T>.GetQueries()
        {
            return _repository.Set<T>();
        }
    }
}
