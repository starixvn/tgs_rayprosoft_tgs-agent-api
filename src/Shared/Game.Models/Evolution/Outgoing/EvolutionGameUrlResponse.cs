using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Evolution.Commons;

namespace TGS.Shared.Game.Models.Evolution.Outgoing
{
    public record EvolutionGameUrlResponse
    {
        [JsonPropertyName("entry")]
        public string Entry { get; set; }
        [JsonPropertyName("entryEmbedded")]
        public string EntryEmbedded { get; set; }
        [JsonPropertyName("errors")]
        public EvolutionError[] Errors { get; set; }
    }
}