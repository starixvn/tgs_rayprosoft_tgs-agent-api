using System.Collections.Generic;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Commands.BatchAdjust;
public record SaveBatchCommand(IAsyncEnumerable<BatchAdjustBalanceItem> Batch);
