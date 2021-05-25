using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Common;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingKickPlayerRequest : BigGamingParamsDigestRequest
    {
        /// <summary>
        /// 登录ID.
        /// </summary>
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
    }
}
