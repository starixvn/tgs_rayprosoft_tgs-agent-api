using System;

namespace TGS.Core.Models.Bonus
{
    public record AgentDividend(Guid BrandId, Guid AgentId, decimal BonusAmount, DateTimeOffset From,
        DateTimeOffset To);
}