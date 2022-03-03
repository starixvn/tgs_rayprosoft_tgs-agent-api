namespace TGS.Apis.AgentApi.Models
{
    public record AgentContributionTotal(
        decimal DepositAmount,
        decimal WithdrawalAmount,
        decimal AdjustmentAmount,
        decimal BonusAmount,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal Turnover,
        decimal WinLossAmount,
        decimal NetProfit);
}
