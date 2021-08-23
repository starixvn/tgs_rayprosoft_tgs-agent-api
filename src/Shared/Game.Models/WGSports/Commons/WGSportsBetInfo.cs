using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.WGSports.Commons
{
    public record WGSportsBetInfo
    {
        [JsonPropertyName("member_id")]
        public string MemberId { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("record_date")]
        public string RecordDate { get; set; }
        [JsonPropertyName("reference")]
        public string Reference { get; set; }
        [JsonPropertyName("sport_type")]
        public string SportType { get; set; }
        [JsonPropertyName("bet_type")]
        public string BetType { get; set; }
        [JsonPropertyName("match_name")]
        public string MatchName { get; set; }
        [JsonPropertyName("home_name")]
        public string HomeName { get; set; }
        [JsonPropertyName("away_name")]
        public string AwayName { get; set; }
        [JsonPropertyName("bet_content")]
        public string BetContent { get; set; }
        [JsonPropertyName("bet_amount")]
        public string BetAmount { get; set; }
        [JsonPropertyName("bet_status")]
        public string BetStatus { get; set; }
        [JsonPropertyName("result")]
        public string Result { get; set; }
        [JsonPropertyName("active_amount")]
        public string ActiveAmount { get; set; }
        [JsonPropertyName("result_score")]
        public string ResultScore { get; set; }
        [JsonPropertyName("settle_date")]
        public string SettleDate { get; set; }
        [JsonPropertyName("bet_ip")]
        public string BetIp { get; set; }
        [JsonPropertyName("init_data")]
        public WGSportsInitData InitData { get; set; }
    }
}
