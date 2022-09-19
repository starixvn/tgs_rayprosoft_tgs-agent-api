using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain;

public interface IBatchTempRepository
{
    ValueTask<Guid> SaveAdjustBalanceBatchAsync(IEnumerable<BatchAdjustBalanceItem> batchAdjustBalanceItems, CancellationToken cancellationToken = default);
}
