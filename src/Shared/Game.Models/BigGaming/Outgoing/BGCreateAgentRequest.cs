using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.BigGaming.Common;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BGCreateAgentRequest : BGSignParamsRequest
    {
        [JsonPropertyName("loginId")]
        public string loginId { get; init; }
        [JsonPropertyName("password")]
        public string password { get; init; }
        [JsonPropertyName("fromIp")]
        public string fromIp { get; init; }

    }
}
