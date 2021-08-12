namespace TGS.Core.Models.GameProviders.Settings.WMCasino
{
    public record WMCasinoAgent
    {
        public string AgentId { get; init; }

        public string CurrencyCode { get; init; }

        public string Signature { get; init; }
    }
}