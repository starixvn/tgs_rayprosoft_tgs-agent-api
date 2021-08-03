using System;

namespace TGS.Services.Operator.Services.GameBalance
{
    public class GameBalanceRecord
    {
        public string From { get; init; } = default!;
        public string To { get; init; } = default!;
        public decimal Amount { get; init; }
        public DateTimeOffset BalanceTime { get; init; }
    }
}