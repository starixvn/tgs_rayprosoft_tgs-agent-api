using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Evolution.Outgoing
{
    public record EvolutionGameUrlResponse
    {
        [JsonPropertyName("entry")]
        public string Entry { get; set; }
        [JsonPropertyName("entryEmbedded")]
        public string EntryEmbedded { get; set; }
    }
}