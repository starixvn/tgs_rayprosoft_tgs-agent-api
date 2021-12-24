using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TF.Outgoing
{
    public class TFCreateMemberRequest
    {
        /// <summary>
        /// 会员每场赛事的限额 (不必须)
        /// </summary>
        [JsonPropertyName("limit_amount_per_event")]
        public decimal? LimitAmountPerEvent { get; set; }
        /// <summary>
        /// 会员ID
        /// </summary>
        [JsonPropertyName("member_code")]
        public string MemberCode { get; set; }
    }
}
