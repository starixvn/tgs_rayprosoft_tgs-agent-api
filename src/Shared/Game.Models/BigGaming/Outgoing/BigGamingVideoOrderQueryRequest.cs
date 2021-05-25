using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.BigGaming.Common;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingOrderQueryRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("loginIds")]
        public string[] LoginIds { get; set; }
        [JsonPropertyName("issueId")]
        public string IssueId { get; set; }
    }
}
