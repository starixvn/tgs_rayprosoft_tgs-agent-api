namespace TGS.Services.Agent;

public record DividendCondition(
    int Rank,
    decimal NetProfit,
    int ActiveNumber,
    decimal DividendPercentage);
