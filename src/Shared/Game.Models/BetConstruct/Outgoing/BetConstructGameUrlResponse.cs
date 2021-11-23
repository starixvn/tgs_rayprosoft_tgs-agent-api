using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BetConstruct.Commons;

namespace TGS.Shared.Game.Models.BetConstruct.Outgoing
{
    public record BetConstructGameUrlResponse
    {
        [JsonPropertyName("entry")]
        public string Entry { get; set; }
        [JsonPropertyName("entryEmbedded")]
        public string EntryEmbedded { get; set; }
        [JsonPropertyName("errors")]
        public BetConstructError[] Errors { get; set; }
    }
}
