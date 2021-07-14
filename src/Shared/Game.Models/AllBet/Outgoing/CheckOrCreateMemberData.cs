using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AllBet.Outgoing
{
    public record CheckOrCreateMemberData
    {
        [JsonPropertyName("player")]
        public string PlayerId { get; init; }
    }
}