namespace TGS.Apis.AgentApi.Models
{
    public record AgentContribution(
        string Username,
        short Type,
        decimal DepositAmount,
        decimal WithdrawalAmount,
        decimal CompanyAdjustAmount,
        decimal AgentAdjustAmount,
        decimal BonusAmount,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal WinLossAmount,
        decimal NetProfit);
}