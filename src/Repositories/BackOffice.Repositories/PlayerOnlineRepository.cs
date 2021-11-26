using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.PlayerOnline;
using TGS.Services.BackOffice.Domain;

namespace TGS.BackOffice.Repositories
{
    internal class PlayerOnlineRepository : IPlayerOnlineRepository
    {
        private readonly IDatabase _database;
        private const string _playerOnlineKey = "playeronline";

        public PlayerOnlineRepository(
            IDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async ValueTask<string[]> GetQueryAsync(Guid brandId)
        {
            var redisDatas = (string[])await _database.ScriptEvaluateAsync(LuaScript.Prepare(
                   //Redis的keys模糊查询：
                   " local res = redis.call('KEYS', @keypattern) " +
                   " return res "), new { @keypattern = $"{ _playerOnlineKey }:{brandId}:*" })
                .ConfigureAwait(true);

            return redisDatas.Select(x => x.Split(':').Last()).ToArray();
        }
    }
}
