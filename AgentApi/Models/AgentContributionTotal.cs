namespace TGS.Apis.AgentApi.Models;

public record AgentContributionTotal(
    decimal DepositAmount,
    decimal DepositHandlingFee,
    decimal WithdrawalAmount,
    decimal CompanyAdjustAmount,
    decimal BonusAdjustAmount,
    decimal AgentAdjustAmount,
    decimal BonusAmount,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal Turnover,
    decimal WinLossAmount,
    decimal NetProfit);