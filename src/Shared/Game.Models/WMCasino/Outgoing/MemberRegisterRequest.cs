using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Outgoing
{
    public record MemberRegisterRequest : BaseRequest
    {
        public override string Action => "MemberRegister";

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("password")]
        public string Password => "TgsP@ssword";

        [JsonPropertyName("username")]
        public string Username { get; init; }

        [JsonPropertyName("timestamp")]
        public long Timestamp => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}