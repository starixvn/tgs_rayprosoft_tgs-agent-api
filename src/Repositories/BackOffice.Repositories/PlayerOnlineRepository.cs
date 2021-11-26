using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Redis;
using TGS.Services.BackOffice.Domain;

namespace TGS.BackOffice.Repositories
{
    internal class PlayerOnlineRepository : IPlayerOnlineRepository
    {
        private readonly IDatabase _database;
        private const string _playerOnlineKey = "playeronline";

        public PlayerOnlineRepository(IDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async ValueTask<string[]> GetQueryAsync(Guid BrandId)
            => (string[])await _database.ScriptEvaluateAsync(LuaScript.Prepare(
                //Redis的keys模糊查询：
                " local res = redis.call('KEYS', @keypattern) " +
                " return res "), new { @keypattern = $"{ _playerOnlineKey }:{BrandId}:*" });
    }
}
