using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.BigGaming.Common;

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
