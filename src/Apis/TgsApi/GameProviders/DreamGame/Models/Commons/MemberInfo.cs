using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons
{
    public record MemberInfo
    {
        [JsonPropertyName("username")]
        public string Username { get; init; }
        [JsonPropertyName("password")]
        public string Password { get; init; }
        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }
        [JsonPropertyName("status")]
        public int Status { get; init; }
        [JsonPropertyName("currency")]
        public string Currency { get; init; }
        [JsonPropertyName("winLimit")]
        public decimal WinLimit { get; init; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }
    }
}
