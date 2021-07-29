using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Commons;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingVideoOrderQueryRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("loginIds")]
        public string[] LoginIds { get; set; }
        [JsonPropertyName("issueId")]
        public string IssueId { get; set; }
    }
}
