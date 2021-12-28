namespace TGS.Core.Models.GameProviders.Settings.Jili
{
    public record JiliMerchant
    {
        public string BrandCode { get; init; }

        public string AgentId { get; init; }

        public string AgentKey { get; init; }
    }
}
