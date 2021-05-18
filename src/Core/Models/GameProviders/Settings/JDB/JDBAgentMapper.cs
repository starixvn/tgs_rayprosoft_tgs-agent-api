namespace TGS.Core.Models.GameProviders.Settings.JDB
{
    public record JDBAgentMapper
    {
        public string CurrencyCode { get; set; }

        public string AgentId { get; set; }

        public bool Status { get; set; }
    }
}
