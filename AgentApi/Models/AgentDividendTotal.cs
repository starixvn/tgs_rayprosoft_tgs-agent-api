namespace TGS.Apis.AgentApi.Models;

public record AgentDividendTotal(
    decimal BonusAdjustAmount,
    decimal AgentAdjustAmount,
    decimal BonusAmount,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal Turnover,
    decimal WinLossAmount,
    decimal NetProfit,
    decimal AgentDividendAmount,
    decimal FromPlayerAgentDividendAmount,
    decimal FromTeamAgentDividendAmount,
    decimal ProductFeeAmount,
    decimal AgentCoveredBonus,
    decimal AgentCoveredBonusAdjust);