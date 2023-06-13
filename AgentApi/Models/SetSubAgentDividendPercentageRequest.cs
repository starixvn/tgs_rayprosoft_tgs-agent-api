using System;

namespace TGS.Apis.AgentApi.Models;

public record SetSubAgentDividendPercentageRequest(Guid SubAgentId, decimal DividendPercentage);
