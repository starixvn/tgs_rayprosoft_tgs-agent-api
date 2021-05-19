using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BigGaming.Common
{
    public record BigGamingBaseErrorResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("sn")]
        public string Sn { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
