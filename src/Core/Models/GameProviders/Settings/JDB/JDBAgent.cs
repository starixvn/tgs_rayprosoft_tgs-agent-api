namespace TGS.Core.Models.GameProviders.Settings.JDB
{
    public record JDBAgent
    {
        public string AESIv { get; init; }

        public string AESKey { get; init; }

        public string ClientDomain { get; init; }

        public string CurrencyCode { get; init; }

        public string Id { get; init; }

        public bool MoreGame { get; init; }
    }
}
