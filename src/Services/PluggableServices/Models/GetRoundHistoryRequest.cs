using System;
using System.Text.Json.Serialization;

namespace TGS.Services.PluggableServices.Models
{
    public record GetRoundHistoryRequest
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("roundid")]
        public Guid RoundId { get; set; }

        /// <summary>
        ///     player's culture code
        /// </summary>
        [JsonPropertyName("language")]
        public string Language { get; init; }
    }
}