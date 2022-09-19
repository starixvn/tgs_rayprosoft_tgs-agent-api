using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Services.BackOffice.Domain;

namespace TGS.BackOffice.Repositories;

internal class RedisBatchTempRepository : IBatchTempRepository
{
    private readonly IDatabase _database;

    public RedisBatchTempRepository(IDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async ValueTask<Guid> SaveAdjustBalanceBatchAsync(
        IEnumerable<BatchAdjustBalanceItem> batchAdjustBalanceItems,
        CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();

        var key = $"BackOffice:Batch:{id}";

        var content = JsonSerializer.Serialize(batchAdjustBalanceItems);

        _ = await _database.StringSetAsync(
            key,
            content,
            expiry: TimeSpan.FromHours(1D),
            when: When.NotExists).ConfigureAwait(false);

        return id;
    }
}
