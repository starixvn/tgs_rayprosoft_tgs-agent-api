namespace TGS.Apis.AgentApi.Models
{
    public record AgentContributionData(
        string Username,
        short Type,
        decimal DepositAmount,
        decimal WithdrawalAmount,
        decimal AdjustmentAmount,
        decimal BonusAmount,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal WinLossAmount,
        decimal NetProfit);
}
