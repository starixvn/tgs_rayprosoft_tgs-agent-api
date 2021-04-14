using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record GetBalanceResponse : BaseResponse
    {
        [JsonPropertyName("userId")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }

        [JsonPropertyName("balanceTs")]
        public string Timestamp { get; init; }
    }
}