using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record BaseResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; init; }

        [JsonPropertyName("msg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Message { get; init; }
    }
}