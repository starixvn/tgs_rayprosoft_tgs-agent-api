using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB
{
    public abstract record BaseResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; init; }

        [JsonPropertyName("err_text")]
        public string ErrorMessage { get; init; }
    }
}