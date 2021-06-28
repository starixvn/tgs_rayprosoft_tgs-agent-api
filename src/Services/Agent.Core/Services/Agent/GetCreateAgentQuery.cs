using System;

namespace TGS.Services.Agent.Services.Agent
{
    public class GetCreateAgentQuery
    {
        public string UserName { get; set; }
        public string Password { get; init; }
        public string RealName { get; init; }
        public Guid ParentAgentId { get; init; }
        public Guid BrandId { get; init; }
        public decimal DividendPercentage { get; init; }
        public decimal LiveDealerRebatePercentage { get; init; }
        public decimal SlotRebatePercentage { get; init; }
        public decimal LotteryRebatePercentage { get; init; }
        public decimal CardGameRebatePercentage { get; init; }
        public decimal SportsRebatePercentage { get; init; }
        public decimal VirtualSportsRebatePercentage { get; init; }
        public string Remarks { get; init; }
    }
}