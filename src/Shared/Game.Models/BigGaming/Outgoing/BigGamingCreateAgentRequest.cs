using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Commons;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingCreateAgentRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("loginId")]
        public string LoginId { get; init; }
        [JsonPropertyName("password")]
        public string Password { get; init; }
        [JsonPropertyName("fromIp")]
        public string FromIp { get; init; }

    }
}
