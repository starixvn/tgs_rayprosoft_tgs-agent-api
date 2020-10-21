using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Log;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    public class LogQueries<T> : ILogQueries<T> where T : class
    {
        private readonly ILogRepository _repository;

        public LogQueries(ILogRepository repository)
        {
            _repository = repository;
        }

        IQueryable<T> ILogQueries<T>.GetQueries()
        {
            return _repository.Set<T>();
        }
        Task<bool> ILogQueries<T>.ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return _repository.Set<T>().AnyAsync(predicate);
        }

        Task<T> ILogQueries<T>.GetSingleAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> preQuery)
        {
            preQuery ??= c => c;
            return preQuery(_repository.Set<T>()).FirstOrDefaultAsync(predicate);
        }
    }
}
