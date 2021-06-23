using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Skywind.Outgoing
{
    public record SkywindGamHistoryRoundDetailsRequest
    {
        [JsonPropertyName("roundId")]
        public string RoundId { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; } = "zh-cn";
    }
}
