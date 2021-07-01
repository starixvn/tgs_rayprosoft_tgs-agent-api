using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.AllBet.Outgoing
{
    public record LoginResponse : ErrorResponse
    {
        [JsonPropertyName("data")]
        public LoginData Data { get; init; }
    }
}