using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FileService.DataService.Model;
using MongoDB.Driver;

namespace FileService.DataService
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

            if (!_mongoClient.GetDatabase(_settings.DatabaseName).ListCollectionNames().ToList()
                .Contains(typeof(T).Name))
            {
                _mongoClient.GetDatabase(_settings.DatabaseName).CreateCollection(typeof(T).Name);
            }
        }

        protected virtual IMongoCollection<T> Collection =>
            _mongoClient.GetDatabase(_settings.DatabaseName).GetCollection<T>(typeof(T).Name);

        public async Task InsertAsync(T obj)
        {
            await Collection.InsertOneAsync(_clientSessionHandle, obj);
        }

        public async Task UpdateAsync(T obj)
        {
            Expression<Func<T, string>> func = f => f.Id;
            var value = (string) obj.GetType().GetProperty(func.Body.ToString().Split(".")[1])?.GetValue(obj, null);
            var filter = Builders<T>.Filter.Eq(func, value);

            await Collection.ReplaceOneAsync(_clientSessionHandle, filter, obj);
        }

        public async Task DeleteAsync(string id)
        {
            await Collection.DeleteOneAsync(_clientSessionHandle, f => f.Id == id);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, id);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetListAsync()
        {
            return await Collection.AsQueryable().ToListAsync();
        }

        public async Task<bool> CanConnection()
        {
            var names = await (await _mongoClient.ListDatabaseNamesAsync()).ToListAsync();
            return names.Count > 0;
        }
    }
}