using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Base.Interfaces
{
    public interface ITgsQueries<T>
    {
        IQueryable<T> GetQueries();
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> preQuery = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
