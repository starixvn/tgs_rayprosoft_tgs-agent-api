using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BigGaming.Common
{
    public record BigGamingParamsDigestRequest
    {
        /// <summary>
        /// 随机字符串，建议使用UUID.
        /// </summary>
        [JsonPropertyName("random")]
        public string Random { get; set; }
        /// <summary>
        /// 签名摘要.
        /// </summary>
        [JsonPropertyName("digest")]
        public string Digest { get; set; }
        /// <summary>
        /// 厅代码.
        /// </summary>
        [JsonPropertyName("sn")]
        public string Sn { get; set; }
    }
}
