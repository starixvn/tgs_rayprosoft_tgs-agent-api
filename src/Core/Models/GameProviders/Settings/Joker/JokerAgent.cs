namespace TGS.Core.Models.GameProviders.Settings.Joker
{
    public record JokerAgent
    {
        public string AgentId { get; init; } = default!;

        public string SecretKey { get; init; } = default!;

        public string CurrencyCode { get; init; } = default!;

        public bool Status { get; init; }
    }
}
