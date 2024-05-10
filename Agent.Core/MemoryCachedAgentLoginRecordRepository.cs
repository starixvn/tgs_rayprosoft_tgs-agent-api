using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace TGS.Services.Agent;

internal class MemoryCachedAgentLoginRecordRepository(
    IAgentLoginRecordRepository loginRecordRepository,
    IMemoryCache memoryCache)
    : IAgentLoginRecordRepository
{
    private readonly IAgentLoginRecordRepository _loginRecordRepository = loginRecordRepository ?? throw new ArgumentNullException(nameof(loginRecordRepository));
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    public async ValueTask<AgentLoginRecordInfo?> GetAgentLoginRecordByIdAsync(Guid loginRecordId)
    {
        var key = $"Tgs:Agent:LoginRecord:{loginRecordId}";

        var loginRecord = await _memoryCache.GetOrCreateAsync(
            key,
            async entry =>
            {
                var loginRecordInfo = await _loginRecordRepository.GetAgentLoginRecordByIdAsync(loginRecordId);

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10D);

                return loginRecordInfo;
            });

        return loginRecord;
    }
}
