namespace TGS.Services.BackOffice.Application.Models.ReportVip
{
    public record VipReportTotalModel(
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