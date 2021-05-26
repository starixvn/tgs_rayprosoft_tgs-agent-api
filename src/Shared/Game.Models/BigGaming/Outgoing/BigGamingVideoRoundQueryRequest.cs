using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Common;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingVideoRoundQueryRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("round")]
        public string Round { get; set; }
    }
}
