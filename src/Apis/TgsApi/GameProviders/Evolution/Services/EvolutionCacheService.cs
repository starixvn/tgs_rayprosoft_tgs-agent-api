using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services;

public class EvolutionCacheService : IEvolutionCacheService
{
    private const string _keyExternalTransactionId = "Evolution:Transaction:{0}:{1}";
    private const string _keySid = "Evolution:Sid:{0}";
    private readonly IDatabase _database;

    public EvolutionCacheService(IDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async ValueTask AddExternalTransactionIdAsync(string playerCode, string externalTransactionId)
    {
        _ = await _database.StringSetAsync(
               string.Format(_keyExternalTransactionId, playerCode, externalTransactionId),
               JsonSerializer.Serialize(new { LogOn = DateTime.UtcNow }),
               TimeSpan.FromDays(1)).ConfigureAwait(false);
    }

    public async ValueTask AddSidAsync(string sid, string playerCode)
    {
        _ = await _database.StringSetAsync(
            string.Format(_keySid, sid),
            playerCode,
            TimeSpan.FromDays(1)).ConfigureAwait(false);
    }

    public Task<bool> ExistExternalTransactionIdAsync(string playerCode, string externalTransactionId)
        => _database.KeyExistsAsync(string.Format(_keyExternalTransactionId, playerCode, externalTransactionId));

    public async ValueTask<string> GetPlayerCodeAsync(string sid)
    {
        var playerCode = await _database.StringGetAsync(string.Format(_keySid, sid)).ConfigureAwait(false);

        return playerCode.ToString();
    }
}
