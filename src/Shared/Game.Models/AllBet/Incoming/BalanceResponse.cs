using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AllBet.Incoming
{
    public record BalanceResponse : ErrorResponse
    {
        [JsonPropertyName("balance")]
        public decimal? Balance { get; init; }

        [JsonPropertyName("version")]
        public long? Version { get; init; }
    }
}