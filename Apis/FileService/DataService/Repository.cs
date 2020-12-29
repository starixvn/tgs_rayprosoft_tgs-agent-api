using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TGS.Apis.FileService.DataService.Model;

namespace TGS.Apis.FileService.DataService
{
    [ExcludeFromCodeCoverage]
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IClientSessionHandle _clientSessionHandle;
        private readonly IMongoClient _mongoClient;
        private readonly IDatabaseSettings _settings;

        public Repository(IDatabaseSettings settings, IMongoClient mongoClient,
            IClientSessionHandle clientSessionHandle)
        {
            (_settings, _mongoClient, _clientSessionHandle) =
                (settings, mongoClient, clientSessionHandle);

            var list = _mongoClient.GetDatabase(_settings.DatabaseName).ListCollectionNames().ToList();
            if (!list
                .Contains(typeof(T).Name))
            {
                _mongoClient.GetDatabase(_settings.DatabaseName).CreateCollection(typeof(T).Name);
            }
        }

        protected virtual IMongoCollection<T> Collection =>
            _mongoClient.GetDatabase(_settings.DatabaseName).GetCollection<T>(typeof(T).Name);

        Task IRepository<T>.InsertAsync(T obj) => Collection.InsertOneAsync(_clientSessionHandle, obj);

        Task IRepository<T>.UpdateAsync(T obj)
        {
            Expression<Func<T, string>> func = f => f.Id;
            var value = (string) obj.GetType().GetProperty(func.Body.ToString().Split(".")[1])?.GetValue(obj, null);
            var filter = Builders<T>.Filter.Eq(func, value);

            return Collection.ReplaceOneAsync(_clientSessionHandle, filter, obj);
        }

        Task IRepository<T>.DeleteAsync(string id)
        {
            return Collection.DeleteOneAsync(_clientSessionHandle, f => f.Id == id);
        }

        Task<T> IRepository<T>.GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, id);
            return Collection.Find(filter).FirstOrDefaultAsync();
        }

        async Task<bool> IRepository<T>.CanConnection()
        {
            var names = await _mongoClient.ListDatabaseNamesAsync();
            return await names.AnyAsync();
        }

        Task IRepository<T>.DeleteManyAsync(string[] ids)
        {
            return Collection.DeleteManyAsync(x => ids.Contains(x.Id)).ContinueWith(x => x.Result.DeletedCount);
        }

        Task<IAsyncCursor<T>> IRepository<T>.GetListAsync(Expression<Func<T, bool>> expression) =>
            Collection.FindAsync<T>(expression);

        IMongoQueryable<T> IRepository<T>.GetQueries() => Collection.AsQueryable();

        Task<T> IRepository<T>.GetByFilterAsync(FilterDefinition<T> filter) =>
            Collection.Find(filter).FirstOrDefaultAsync();

        Task<List<T>> IRepository<T>.GetListByFilterAsync(FilterDefinition<T> filter) =>
            Collection.Find(filter).ToListAsync();
    }
}