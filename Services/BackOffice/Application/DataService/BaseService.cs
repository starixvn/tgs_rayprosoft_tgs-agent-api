using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;

namespace TGS.Services.BackOffice.Application.DataService
{
    public abstract class BaseService
    {
        private readonly ITgsRepository _repository;

        protected BaseService(ITgsRepository tgsRepository)
        {
            _repository = tgsRepository;
        }

        protected Task ChangeStatusAsync<T>(T entity) where T : class
        {
            _repository.Entry(entity).State = EntityState.Modified;

            return _repository.SaveChangesAsync();
        }

        protected Task<T> GetEntityFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> preQuery, bool tracking) where T : class
        {
            var entity = tracking ? _repository.Set<T>() : _repository.Set<T>().AsNoTracking();
            preQuery ??= c => c;

            return preQuery(entity).FirstOrDefaultAsync(predicate);
        }

        protected IQueryable<T> GetQueriesAsync<T>(bool tracking) where T : class
        {
            return tracking ? _repository.Set<T>() : _repository.Set<T>().AsNoTracking();
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

        protected Task CreateOrUpdateEntitysAsync<T>(T[] entities) where T : class
        {
            _repository.BulkInsertOrUpdateAsync(entities);
            return _repository.SaveChangesAsync();
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