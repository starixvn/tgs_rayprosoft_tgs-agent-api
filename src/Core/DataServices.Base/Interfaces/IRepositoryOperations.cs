using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Base.Interfaces
{
    public interface IRepositoryOperations
    {
        public Task<T> GetEntityFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> preQuery) where T : class;

        public IQueryable<T> GetQueriesAsync<T>(Func<IQueryable<T>, IQueryable<T>> preQuery, bool tracking)
            where T : class;

        public Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        public Task CreateEntityAsync<T>(T entity) where T : class;

        public Task CreateEntitiesAsync<T>(IEnumerable<T> entities) where T : class;

        public Task UpdateEntitiesAsync<T>(IEnumerable<T> entities) where T : class;

        public Task UpdateEntityAsync<T>(T entity) where T : class;

        public Task DeleteEntityAsync<T>(T entity) where T : class;

        public Task DeleteEntitiesAsync<T>(IEnumerable<T> entities) where T : class;
    }
}