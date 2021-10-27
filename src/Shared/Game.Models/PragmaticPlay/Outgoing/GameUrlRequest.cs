using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.PragmaticPlay.Common;

namespace TGS.Shared.Game.Models.PragmaticPlay.Outgoing
{
    public record GameUrlRequest : HashRequest
    {
        [JsonPropertyName("symbol")]
        public string ExternalGameId { get; init; }

        [JsonPropertyName("language")]
        public string Language { get; init; }

        [JsonPropertyName("lobbyUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LobbyUrl { get; init; }

        [JsonPropertyName("platform")]
        public string Platform { get; init; }

        [JsonPropertyName("secureLogin")]
        public string SecureLogin { get; init; }

        [JsonPropertyName("technology")]
        public string Technology => "H5";

        [JsonPropertyName("token")]
        public string Token { get; init; }
    }
}