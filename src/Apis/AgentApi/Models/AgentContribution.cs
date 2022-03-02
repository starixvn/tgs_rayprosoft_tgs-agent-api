namespace TGS.Apis.AgentApi.Models
{
    public record AgentContribution(
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
