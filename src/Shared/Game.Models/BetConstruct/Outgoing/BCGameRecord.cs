using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Outgoing
{
    public class BCGameRecord
    {
        [JsonPropertyName("SelectionId")]
        public long SelectionId { get; init; }

        [JsonPropertyName("State")]
        public int State { get; init; }
    }
}
