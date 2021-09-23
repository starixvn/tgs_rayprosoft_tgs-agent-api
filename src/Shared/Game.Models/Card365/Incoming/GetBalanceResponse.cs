using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Incoming
{
    public record BalanceResponse : ErrorResponse
    {
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
    }
}