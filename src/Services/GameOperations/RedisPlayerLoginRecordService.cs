using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Models.Enums;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;

namespace TGS.Services.GameOperations
{
    internal class RedisPlayerLoginRecordService : IRedisPlayerLoginRecordService
    {
        private readonly IDatabase _database;
        private readonly IPlayerLoginRecordRepository _playerLoginRecordRepository;
        private readonly string _redisKey = "GameProvider:PlayerLoginRecords:{0}";

        public RedisPlayerLoginRecordService(
            IDatabase database,
            IPlayerLoginRecordRepository playerLoginRecordRepository
            )
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _playerLoginRecordRepository = playerLoginRecordRepository ?? throw new ArgumentNullException(nameof(playerLoginRecordRepository));
        }

        public async Task<PlayerLoginRecordModel> GetAsync(Guid loginRecordId)
        {
            var key = string.Format(_redisKey, loginRecordId);
            if (await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var value = await _database.StringGetAsync(string.Format(_redisKey, loginRecordId)).ConfigureAwait(false);
                return JsonSerializer.Deserialize<PlayerLoginRecordModel>(value);
            }
            var data = await _playerLoginRecordRepository.GetAsync(loginRecordId).ConfigureAwait(false);
            if (data == null)
            {
                return null;
            }
            await _database.StringSetAsync(key, JsonSerializer.Serialize(data), TimeSpan.FromMinutes(1)).ConfigureAwait(false);
            return data;
        }

        public async Task<PlayerLoginRecordModel> GetLastAsync(string playerCode)
        {
            var datas = await CachedQueryEnabledAsync(playerCode).ConfigureAwait(false);
            return datas.FirstOrDefault();
        }

        public async Task<PlayerLoginRecordModel> GetLastAsync(string playerCode, PlatformType platform)
        {
            var datas = await CachedQueryEnabledAsync(playerCode).ConfigureAwait(false);
            return datas.Where(x => x.PlatformType == platform).FirstOrDefault();
        }

        public Task<PlayerLoginRecordModel[]> QueryEnabledAsync(string playerCode)
            => CachedQueryEnabledAsync(playerCode);

        private async Task<PlayerLoginRecordModel[]> CachedQueryEnabledAsync(string playerCode)
        {
            var key = string.Format(_redisKey, playerCode);

            if (await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var values = await _database.HashGetAsync(key, "enabled").ConfigureAwait(false);
                return JsonSerializer.Deserialize<PlayerLoginRecordModel[]>(values);
            }
            var datas = await _playerLoginRecordRepository.QueryEnabledAsync(playerCode).ConfigureAwait(false);

            await _database.HashSetAsync(key,
                new HashEntry[]
                {
                    new( "enabled",JsonSerializer.Serialize(datas))
                });

            await _database.KeyExpireAsync(key, TimeSpan.FromMinutes(1));

            return datas;
        }
    }
}
