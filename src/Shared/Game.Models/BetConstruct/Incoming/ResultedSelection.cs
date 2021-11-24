using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Incoming
{
    public class ResultedSelection
    {
        [JsonPropertyName("SelectionId")]
        public long SelectionId { get; init; }

        [JsonPropertyName("State")]
        public int State { get; init; }
    }
}
