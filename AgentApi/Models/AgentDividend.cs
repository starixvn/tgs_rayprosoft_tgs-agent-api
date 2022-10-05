using System;

namespace TGS.Apis.AgentApi.Models;

public record AgentDividend(
    Guid Id,
    string Username,
    decimal AgentAdjustAmount,
    decimal BonusAdjustAmount,
    decimal BonusAmount,
    decimal BetAmount,
    decimal ValidBetAmount,
    decimal Turnover,
    decimal WinLossAmount,
    decimal NetProfit,
    decimal AgentDividendAmount,
    decimal FromPlayerAgentDividendAmount,
    decimal FromTeamAgentDividendAmount,
    decimal AgentDividendPercentage,
    decimal? AgentCoveredBonus,
    decimal? AgentCoveredBonusAdjust);