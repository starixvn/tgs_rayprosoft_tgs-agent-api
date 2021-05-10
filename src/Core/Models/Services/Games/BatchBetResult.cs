using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games
{
    public record BatchBetResult
    {
        public TgsApiError Error { get; init; }
        public string ErrorMessage { get; init; }
        public string ExternalBatchId { get; init; }
        public bool IsDuplicate { get; init; }
        public DateTimeOffset BatchTime { get; init; }
        public int TransactionsCount { get; init; }
        public BatchBetResultData[] Errors { get; init; }
        public BatchBetResultData[] Results { get; init; }
        public Dictionary<string, decimal> PlayerBalances { get; init; }
    }
}