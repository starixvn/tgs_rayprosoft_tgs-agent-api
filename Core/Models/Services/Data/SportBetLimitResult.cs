namespace TGS.Core.Models.Services.Data
{
    public record SportBetLimitResult
    {
        public string SportTypeExternalId { get; init; }

        public string MarketTypeExternalId { get; init; }

        public int MinBet { get; init; }

        public int MaxBet { get; init; }

        public int MaxBetPerMatch { get; init; }
    }
}