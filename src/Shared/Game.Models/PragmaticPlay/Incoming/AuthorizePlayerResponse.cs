using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record AuthorizePlayerResponse : BalanceResponse
    {
        [JsonPropertyName("currency")]
        public string Currency { get; init; }

        [JsonPropertyName("userId")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("token")]
        public string Token { get; init; }
    }
}