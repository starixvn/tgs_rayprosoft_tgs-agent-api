namespace TGS.Apis.AgentApi.Models
{
    public record AgentDividendTotal(
        decimal DepositAmount,
        decimal WithdrawalAmount,
        decimal CompanyAdjustAmount,
        decimal AgentAdjustAmount,
        decimal BonusAmount,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal Turnover,
        decimal WinLossAmount,
        decimal NetProfit,
        decimal AgentDividendAmount,
        decimal FromPlayerAgentDividendAmount,
        decimal FromTeamAgentDividendAmount);
}