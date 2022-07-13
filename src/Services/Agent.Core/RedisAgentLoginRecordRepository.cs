using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Services.Agent;

internal class RedisAgentLoginRecordRepository : IAgentLoginRecordRepository
{
    private static readonly TimeSpan _Expiry = TimeSpan.FromHours(1D);

    private static readonly JsonSerializerOptions _SerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IAgentLoginRecordRepository _loginRecordRepository;
    private readonly IDatabase _redisDatabase;

    public RedisAgentLoginRecordRepository(
        IAgentLoginRecordRepository loginRecordRepository,
        IDatabase redisDatabase)
    {
        _loginRecordRepository = loginRecordRepository ?? throw new ArgumentNullException(nameof(loginRecordRepository));
        _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
    }

    public async ValueTask<AgentLoginRecordInfo?> GetAgentLoginRecordByIdAsync(Guid loginRecordId)
    {
        var key = $"Tgs:Agent:LoginRecord:{loginRecordId}";

        var json = await _redisDatabase.StringGetAsync(key).ConfigureAwait(false);

        if (json.HasValue)
            return JsonSerializer.Deserialize<AgentLoginRecordInfo>(((string?)json)!, _SerializerOptions);

        var loginRecord = await _loginRecordRepository
            .GetAgentLoginRecordByIdAsync(loginRecordId)
            .ConfigureAwait(false);

        var value = JsonSerializer.Serialize(loginRecord, _SerializerOptions);

        _ = await _redisDatabase.StringSetAsync(
            key,
            value,
            _Expiry,
            when: When.NotExists).ConfigureAwait(false);

        return loginRecord;
    }
}
