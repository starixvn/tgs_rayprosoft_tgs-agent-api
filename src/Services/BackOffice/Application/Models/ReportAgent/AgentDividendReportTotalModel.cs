namespace TGS.Services.BackOffice.Application.Models.ReportAgent
{
    public record AgentDividendReportTotalModel(
        decimal DepositAmount,
        decimal WithdrawalAmount,
        decimal AdjustmentAmount,
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