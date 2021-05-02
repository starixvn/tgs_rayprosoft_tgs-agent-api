using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB
{
    public abstract class BaseResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("err_text")]
        public string ErrorMessage { get; set; }
    }
}