using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Incoming
{
    public record GetBalanceRequest
    {
        [JsonPropertyName("action")]
        public string Action { get; init; }

        [JsonPropertyName("userId")]
        public string PlayerCode { get; init; }
    }
}