using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.WGSports.Commons
{
    public record WGSportsInitData
    {
        [JsonPropertyName("FB")]
        public string FB { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("valid")]
        public string Valid { get; set; }
        [JsonPropertyName("live_status")]
        public string Live_status { get; set; }
        [JsonPropertyName("plan_no")]
        public string Plan_no { get; set; }
        [JsonPropertyName("part")]
        public string Part { get; set; }
        [JsonPropertyName("win")]
        public string Win { get; set; }
        [JsonPropertyName("odds_typewin")]
        public string OddsType { get; set; }
        [JsonPropertyName("match_date")]
        public string MatchDate { get; set; }
    }
}
