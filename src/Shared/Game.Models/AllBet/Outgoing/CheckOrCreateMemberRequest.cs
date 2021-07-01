using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AllBet.Outgoing
{
    public record CheckOrCreateMemberRequest
    {
        [JsonPropertyName("agent")]
        public string AgentId { get; init; }

        [JsonPropertyName("player")]
        public string PlayerId { get; init; }
    }
}