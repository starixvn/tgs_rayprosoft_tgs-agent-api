using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AllBet.Incoming
{
    public record BalanceResponse
    {
        [JsonPropertyName("resultCode")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("message")]
        public string ErrorMessage { get; init; } = "Success";

        [JsonPropertyName("balance")]
        public decimal? Balance { get; init; }

        [JsonPropertyName("version")]
        public long? Version { get; init; }
    }
}