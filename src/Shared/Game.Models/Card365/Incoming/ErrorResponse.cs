using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Card365.Incoming
{
    public record ErrorResponse
    {
        [JsonPropertyName("status")]
        public string ErrorCode { get; init; }

        [JsonPropertyName("msg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ErrorMessage { get; init; }
    }
}