using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record GetBalanceResponse : BalanceResponse
    {
        [JsonPropertyName("userId")]
        public string UserId { get; init; }

        [JsonPropertyName("balanceTs")]
        public string Timestamp { get; init; }
    }
}