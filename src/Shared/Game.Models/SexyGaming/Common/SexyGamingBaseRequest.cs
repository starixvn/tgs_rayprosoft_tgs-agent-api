using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.SexyGaming.Common
{
    public record SexyGamingBaseRequest
    {
        [JsonPropertyName("cert")]
        public string Cert { get; set; }
        [JsonPropertyName("agentId")]
        public string AgentId { get; set; }
    }
}
