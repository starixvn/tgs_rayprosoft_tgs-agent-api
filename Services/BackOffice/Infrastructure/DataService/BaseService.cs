using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public abstract class BaseService
    {
        private readonly ITgsRepository _repository;

        protected BaseService(ITgsRepository tgsRepository)
        {
            _repository = tgsRepository;
        }

        protected Task<T> GetEntityFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> preQuery) where T : class
        {
            preQuery ??= c => c;

            return preQuery(_repository.Set<T>()).FirstOrDefaultAsync(predicate);
        }

        protected IQueryable<T> GetQueriesAsync<T>(Func<IQueryable<T>, IQueryable<T>> preQuery, bool tracking)
            where T : class
        {
            preQuery ??= c => c;
            return tracking ? preQuery(_repository.Set<T>()) : preQuery(_repository.Set<T>()).AsNoTracking();
        }

        protected Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _repository.Set<T>().AnyAsync(predicate);
        }

        protected Task CreateEntityAsync<T>(T entity) where T : class
        {
            _repository.Set<T>().AddAsync(entity);
            return _repository.SaveChangesAsync();
        }

        protected async Task UpdateEntitiesAsync<T>(IEnumerable<T> entities) where T : class
        {
            _repository.Set<T>().UpdateRange(entities);
            await _repository.SaveChangesAsync();
        }

        protected async Task CreateOrUpdateEntitiesAsync<T>(T[] entities) where T : class
        {
            await _repository.BulkInsertOrUpdateAsync(entities);
            await _repository.SaveChangesAsync();
        }

        protected Task UpdateEntityAsync<T>(T entity) where T : class
        {
            _repository.Entry(entity).State = EntityState.Modified;
            return _repository.SaveChangesAsync();
        }

        protected Task DeleteEntityAsync<T>(T entity) where T : class
        {
            _repository.Entry(entity).State = EntityState.Deleted;
            return _repository.SaveChangesAsync();
        }

        protected Task DeleteEntitiesAsync<T>(IEnumerable<T> entities) where T : class
        {
            _repository.Set<T>().RemoveRange(entities);
            return _repository.SaveChangesAsync();
        }
    }
}