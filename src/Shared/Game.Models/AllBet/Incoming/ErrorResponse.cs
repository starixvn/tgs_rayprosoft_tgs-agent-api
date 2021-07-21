using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AllBet.Incoming
{
    public record ErrorResponse
    {
        [JsonPropertyName("resultCode")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("message")]
        public string ErrorMessage { get; init; }
    }
}