using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services;

public class JiliRedisDataStore : IJiliDataStore
{
    private readonly IDatabase _database;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JiliRedisDataStore(IDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false
        };
    }

    public async ValueTask<bool> CheckCancelledRoundAsync(string externalRoundId)
    {
        var dataKey = $"Jili:CancelledRound:{externalRoundId}";

        return await _database.KeyExistsAsync(dataKey).ConfigureAwait(false);
    }

    public async ValueTask<bool> SetCancelledRoundAsync(string externalRoundId, JiliCancelTransactionRequest request)
    {
        var dataKey = $"Jili:CancelledRound:{externalRoundId}";

        var value = JsonSerializer.Serialize(request, _jsonSerializerOptions);
        var expiry = TimeSpan.FromDays(1);

        return await _database
            .StringSetAsync(dataKey, value, expiry, when: When.NotExists)
            .ConfigureAwait(false);
    }
}
