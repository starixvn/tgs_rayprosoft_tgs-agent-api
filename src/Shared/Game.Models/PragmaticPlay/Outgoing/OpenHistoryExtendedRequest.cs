using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.PragmaticPlay.Common;

namespace TGS.Shared.Game.Models.PragmaticPlay.Outgoing
{
    public record OpenHistoryExtendedRequest : HashRequest
    {
        [JsonPropertyName("secureLogin")]
        public string SecureLogin { get; init; }

        [JsonPropertyName("roundId")]
        public string RoundId { get; init; }

        [JsonPropertyName("playerId")]
        public string playerCode { get; init; }

        [JsonPropertyName("language")]
        public string Language { get; init; }
    }
}