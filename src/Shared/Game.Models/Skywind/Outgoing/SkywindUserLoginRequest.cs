using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Skywind.Outgoing
{
    public record SkywindUserLoginRequest
    {
        [JsonPropertyName("secretKey")]
        public string SecretKey { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
