using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain;

public interface IBatchTempRepository
{
    IAsyncEnumerable<BatchAdjustBalanceItem> GetAdjustBalanceBatchAsync(Guid batchId, CancellationToken cancellationToken = default);

    ValueTask<Guid> SaveAdjustBalanceBatchAsync(IEnumerable<BatchAdjustBalanceItem> batchAdjustBalanceItems, CancellationToken cancellationToken = default);
}
