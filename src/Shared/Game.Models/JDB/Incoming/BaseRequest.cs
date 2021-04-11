using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record BaseRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("dtsent")]
        public DateTimeOffset SentOn { get; init; }

        [JsonPropertyName("auth")]
        public Authorization Authorization { get; init; }

        [JsonPropertyName("basegame")]
        public Game Game { get; init; }
    }

    public record Authorization
    {
        [JsonPropertyName("username")]
        public string ClientId { get; init; }

        [JsonPropertyName("passkey")]
        public string SecretKey { get; init; }

        [JsonPropertyName("machinename")]
        public string MachineName { get; init; }

        [JsonPropertyName("brandid")]
        public string HabaneroBrandId { get; init; }

        [JsonPropertyName("locale")]
        public string Culture { get; init; }
    }

    public record Game
    {
        [JsonPropertyName("brandgameid")]
        public string ExternalRtpId { get; init; }

        [JsonPropertyName("keyname")]
        public string ExternalId { get; init; }
    }
}