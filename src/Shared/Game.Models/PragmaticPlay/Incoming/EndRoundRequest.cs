using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record EndRoundRequest : BaseRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; init; }

        [JsonPropertyName("gameId")]
        public string GameId { get; init; }

        [JsonPropertyName("roundId")]
        public string RoundId { get; init; }
    }
}