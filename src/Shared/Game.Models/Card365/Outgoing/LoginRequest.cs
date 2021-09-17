using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Outgoing
{
    public record LoginRequest
    {
        [JsonPropertyName("user")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("key")]
        public string AccessKey { get; init; }

        [JsonPropertyName("extension1")]
        public string AgentId { get; init; }

        [JsonPropertyName("extension3")]
        public string Token { get; init; }

        [JsonPropertyName("userName")]
        public string Username { get; init; }

        [JsonPropertyName("language")]
        public string Language { get; init; }

        [JsonPropertyName("jackpot")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public int EnableJackpotValue => EnableJackpot ? 1 : 0;

        [JsonPropertyName("blockfree")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public int EnableFreeGameValue => EnableFreeGame ? 0 : 1;

        [JsonPropertyName("enableReward")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public int EnableRewardValue => EnableReward ? 1 : 0;

        [JsonPropertyName("fullscreen")]
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public int EnableFullscreenValue => EnableFullscreen ? 1 : 0;

        [JsonIgnore]
        public bool EnableJackpot { get; init; }

        [JsonIgnore]
        public bool EnableFreeGame { get; init; }

        [JsonIgnore]
        public bool EnableReward { get; init; }

        [JsonIgnore]
        public bool EnableFullscreen { get; init; }
    }
}