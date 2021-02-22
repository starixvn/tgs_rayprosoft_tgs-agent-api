using System;
using System.Collections.Generic;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.GameOperations.Models.Data
{
    internal class BatchBetPartitionState
    {
        internal BatchBetPartitionState(Guid batchId)
        {
            BatchId = batchId;
        }

        internal Guid BatchId { get; }
        internal int Successes { get; set; } = 0;
        internal Dictionary<string, decimal> PlayerBalances { get; } = new();
        internal List<BatchBetResultError> Errors { get; } = new();
    }
}