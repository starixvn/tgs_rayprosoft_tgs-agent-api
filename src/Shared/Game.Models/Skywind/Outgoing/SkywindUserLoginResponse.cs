using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Skywind.Outgoing
{
    public record SkywindUserLoginResponse
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("lastPasswordUpdate")]
        public DateTime LastPasswordUpdate { get; set; }
    }
}
