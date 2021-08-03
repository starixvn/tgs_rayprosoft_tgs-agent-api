namespace TGS.Apis.AgentApi.Models
{
    public class CreateAgentRequest
    {
        public string UserName { get; set; }

        public string Password { get; init; }

        public string RealName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

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