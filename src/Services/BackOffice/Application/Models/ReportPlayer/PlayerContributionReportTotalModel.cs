namespace TGS.Services.BackOffice.Application.Models.ReportPlayer
{
    public record PlayerContributionReportTotalModel(
        decimal DepositAmount,
        decimal WithdrawalAmount,
        decimal AdjustmentAmount,
        decimal BonusAmount,
        decimal BetAmount,
        decimal ValidBetAmount,
        decimal Turnover,
        decimal WinLossAmount,
        decimal NetProfit
    );
}