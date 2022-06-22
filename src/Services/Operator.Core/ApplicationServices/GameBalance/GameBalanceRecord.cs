using System;

namespace TGS.Services.Operator.ApplicationServices.GameBalance;

public class GameBalanceRecord
{
    public string From { get; init; } = default!;
    public string To { get; init; } = default!;
    public decimal Amount { get; init; }
    public DateTimeOffset BalanceTime { get; init; }
}
