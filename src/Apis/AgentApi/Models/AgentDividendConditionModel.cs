namespace TGS.Apis.AgentApi.Models
{
    public class AgentDividendConditionModel
    {
        public int ActiveNumber { get; set; }

        public decimal DividendPercentage { get; set; }

        public decimal NetProfit { get; set; }

        public int Rank { get; set; }
    }
}
