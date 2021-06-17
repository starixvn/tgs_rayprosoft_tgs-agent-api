using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Skywind.Outgoing
{
    public record SkywindGameUrlResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
