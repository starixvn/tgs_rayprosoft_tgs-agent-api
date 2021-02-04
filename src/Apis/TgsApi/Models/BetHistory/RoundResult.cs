using System;
using TGS.Core.Models.Services.BetHistories;

namespace TGS.Apis.TgsApi.Models.BetHistory
{
    public record RoundResult
    {
        public decimal BetAmount { get; init; }
        public decimal Turnover { get; init; }
        public decimal Winloss { get; init; }
        public string GameResult { get; init; }
        public IData[] Data { get; init; } = Array.Empty<IData>();
    }
}