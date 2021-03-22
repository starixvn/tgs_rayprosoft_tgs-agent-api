using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record AuthorizePlayerResponse : BalanceResponse
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("userId")]
        public string PlayerCode { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}