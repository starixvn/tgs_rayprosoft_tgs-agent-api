using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
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

            if (!_mongoClient.GetDatabase(_settings.DatabaseName).ListCollectionNames().ToList()
                .Contains(typeof(T).Name))
            {
                _mongoClient.GetDatabase(_settings.DatabaseName).CreateCollection(typeof(T).Name);
            }
        }

        protected virtual IMongoCollection<T> Collection =>
            _mongoClient.GetDatabase(_settings.DatabaseName).GetCollection<T>(typeof(T).Name);

        public Task InsertAsync(T obj)
        {
            return Collection.InsertOneAsync(_clientSessionHandle, obj);
        }

        public async Task UpdateAsync(T obj)
        {
            Expression<Func<T, string>> func = f => f.Id;
            var value = (string) obj.GetType().GetProperty(func.Body.ToString().Split(".")[1])?.GetValue(obj, null);
            var filter = Builders<T>.Filter.Eq(func, value);

            await Collection.ReplaceOneAsync(_clientSessionHandle, filter, obj);
        }

        public Task DeleteAsync(string id)
        {
            return Collection.DeleteOneAsync(_clientSessionHandle, f => f.Id == id);
        }

        public Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, id);
            return Collection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<T[]> GetListAsync()
        {
            return Collection.AsQueryable().ToArrayAsync();
        }

        public async Task<bool> CanConnection()
        {
            var names = await _mongoClient.ListDatabaseNamesAsync();
            return await names.AnyAsync();
        }
    }
}