namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record PlaceBetRequest : BaseRequest
    {
        public string UserId { get; init; }

        public string GameId { get; init; }

        public string RoundId { get; init; }

        public decimal Amount { get; init; }

        public string Reference { get; init; }

        public long Timestamp { get; init; }

        public string RoundDetails { get; init; }
    }
}