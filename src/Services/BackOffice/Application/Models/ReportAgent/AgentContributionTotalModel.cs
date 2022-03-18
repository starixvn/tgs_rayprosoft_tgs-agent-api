namespace TGS.Services.BackOffice.Application.Models.ReportAgent
{
    public record AgentContributionTotalModel(
        decimal DepositAmount,
        decimal WithdrawalAmount,
        decimal CompanyAdjustAmount,
        decimal AgentAdjustAmount,
        decimal BonusAmount,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal Turnover,
        decimal WinLossAmount,
        decimal NetProfit);
}