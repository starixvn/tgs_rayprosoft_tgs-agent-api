using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Core.DataServices.Base
{
    public abstract class BaseRepositoryOperations<TRepository> : IRepositoryOperations
        where TRepository : IBaseRepository
    {
        private readonly TRepository _repository;

        protected BaseRepositoryOperations(TRepository repository)
        {
            _repository = repository;
        }

        public Task<T> GetEntityFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> preQuery) where T : class
        {
            preQuery ??= c => c;

            return preQuery(_repository.Set<T>()).FirstOrDefaultAsync(predicate);
        }

        public IQueryable<T> GetQueriesAsync<T>(Func<IQueryable<T>, IQueryable<T>> preQuery,
            bool tracking) where T : class
        {
            preQuery ??= c => c;
            return tracking ? preQuery(_repository.Set<T>()) : preQuery(_repository.Set<T>()).AsNoTracking();
        }

        public Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class =>
            _repository.Set<T>().AnyAsync(predicate);

        public Task CreateEntityAsync<T>(T entity) where T : class
        {
            _repository.Set<T>().AddAsync(entity);
            return _repository.SaveChangesAsync();
        }

        public async Task UpdateEntitiesAsync<T>(IEnumerable<T> entities) where T : class
        {
            _repository.Set<T>().UpdateRange(entities);
            await _repository.SaveChangesAsync();
        }

        public async Task CreateEntitiesAsync<T>(T[] entities) where T : class
        {
            await _repository.BulkInsertAsync(entities);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateEntitiesAsync<T>(T[] entities) where T : class
        {
            await _repository.BulkUpdateAsync(entities);
            await _repository.SaveChangesAsync();
        }

        public Task UpdateEntityAsync<T>(T entity) where T : class
        {
            _repository.Entry(entity).State = EntityState.Modified;
            return _repository.SaveChangesAsync();
        }

        public Task DeleteEntityAsync<T>(T entity) where T : class
        {
            _repository.Entry(entity).State = EntityState.Deleted;
            return _repository.SaveChangesAsync();
        }

        public Task DeleteEntitiesAsync<T>(IEnumerable<T> entities) where T : class
        {
            _repository.Set<T>().RemoveRange(entities);
            return _repository.SaveChangesAsync();
        }
    }
}