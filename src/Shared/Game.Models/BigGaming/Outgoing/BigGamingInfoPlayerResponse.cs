using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingInfoPlayerResponse
    {
        [JsonPropertyName("sn")]
        public string Sn { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }
        [JsonPropertyName("regTime")]
        public string RegTime { get; set; }
        [JsonPropertyName("currency")]
        public int Currency { get; set; }
        [JsonPropertyName("userStatus")]
        public int UserStatus { get; set; }
        [JsonPropertyName("memo")]
        public string Memo { get; set; }
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("regType")]
        public string RegType { get; set; }
        [JsonPropertyName("regIp")]
        public string RegIp { get; set; }
        [JsonPropertyName("loginLastUpdateTime")]
        public DateTime LoginLastUpdateTime { get; set; }
        [JsonPropertyName("loginIp")]
        public string LoginIp { get; set; }
        [JsonPropertyName("loginCount")]
        public int LoginCount { get; set; }
        [JsonPropertyName("remark")]
        public string Remark { get; set; }
    }
}
