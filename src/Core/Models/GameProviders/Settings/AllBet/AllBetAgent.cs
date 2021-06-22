namespace TGS.Core.Models.GameProviders.Settings.AllBet
{
    public record AllBetAgent
    {
        public string AgentId { get; init; }

        public string PropertyId { get; init; }

        public string CurrencyCode { get; init; }

        public string PartnerApiKey { get; init; }

        public string AllBetApiKey { get; init; }

        public string Suffix { get; init; }
    }
}