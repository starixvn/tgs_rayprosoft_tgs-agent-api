using System;
using System.Collections.Generic;
using System.Linq;
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

    public IAsyncEnumerable<BatchAdjustBalanceItem> GetAdjustBalanceBatchAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        var key = $"BackOffice:Batch:{batchId}";

        return _database.StringGetAsync(key)
            .ToAsyncEnumerable()
            .Select(value => JsonSerializer.Deserialize<BatchAdjustBalanceItem[]>(value))
            .Where(arr => arr is not null)
            .SelectMany(arr => arr.ToAsyncEnumerable());
    }

    public IAsyncEnumerable<BatchAdjustBalanceItemResult> GetAdjustBalanceItemResultsAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        var key = $"BackOffice:BatchResult:{batchId}";

        return _database.StringGetAsync(key)
            .ToAsyncEnumerable()
            .Select(value => JsonSerializer.Deserialize<BatchAdjustBalanceItemResult[]>(value))
            .Where(arr => arr is not null)
            .SelectMany(arr => arr.ToAsyncEnumerable());
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

    public async ValueTask SaveAdjustBalanceBatchResultAsync(
        Guid batchId,
        IEnumerable<BatchAdjustBalanceItemResult> results,
        CancellationToken cancellation = default)
    {
        var key = $"BackOffice:BatchResult:{batchId}";

        var content = JsonSerializer.Serialize(results);

        _ = await _database.StringSetAsync(
            key,
            content,
            expiry: TimeSpan.FromHours(1D)).ConfigureAwait(false);
    }
}
