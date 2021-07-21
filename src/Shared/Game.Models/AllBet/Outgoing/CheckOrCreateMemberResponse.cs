using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AllBet.Outgoing
{
    public record CheckOrCreateMemberResponse : ErrorResponse
    {
        [JsonPropertyName("player")]
        public CheckOrCreateMemberData Data { get; init; }
    }
}