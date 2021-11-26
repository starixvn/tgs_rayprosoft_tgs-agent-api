using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Services.Operator;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TGS.Operator.Repositories
{
    internal class PlayerOnlineRepository : IPlayerOnlineRepository
    {
        private readonly ILogger<PlayerOnlineRepository> _logge;
        private readonly IDatabase _database;
        private const string _playerOnlineKey = "playeronline";
        private readonly ILogger<PlayerOnlineRepository> _logger;
        public PlayerOnlineRepository(ILogger<PlayerOnlineRepository> logge, IDatabase database)
        {
            _logge = logge ?? throw new ArgumentNullException(nameof(logge));
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public Task SetAsync(Guid brandId, string playerCode)
        {

            _logge.LogInformation($"{_playerOnlineKey}:{brandId}:{playerCode}");
            return _database.StringSetAsync(
                 $"{_playerOnlineKey}:{brandId}:{playerCode}",
                 JsonSerializer.Serialize(new { Online = DateTimeOffset.UtcNow }),
                 TimeSpan.FromMinutes(1));
        }
    }
}
