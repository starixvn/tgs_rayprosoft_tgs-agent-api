using System;

namespace TGS.Services.Agent.Services.Agent
{
    public class AgentDividendConditionModel
    {
        public int ActiveNumber { get; set; }

        public decimal DividendPercentage { get; set; }

        public decimal NetProfit { get; set; }

        public int Rank { get; set; }
    }

    public class CreateAgentCommand
    {
        public AgentDividendConditionModel[] AgentDividendConditions { get; init; } = default!;

        public Guid BrandId { get; init; }

        public decimal CardGameRebatePercentage { get; init; }

        public string Email { get; init; } = default!;

        public decimal LiveDealerRebatePercentage { get; init; }

        public decimal LotteryRebatePercentage { get; init; }

        public Guid ParentAgentId { get; init; }

        public string Password { get; init; } = default!;

        public string Phone { get; init; } = default!;

        public string RealName { get; init; } = default!;

        public string? Remarks { get; init; }

        public decimal SlotRebatePercentage { get; init; }

        public decimal SportsRebatePercentage { get; init; }

        public string Username { get; set; } = default!;

        public decimal VirtualSportsRebatePercentage { get; init; }
    }
}
