using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Skywind.Common;

namespace TGS.Shared.Game.Models.Skywind.Incoming
{
    public record SkywindBalanceRequest : SkywindSingleWalletRequest
    {
        [JsonPropertyName("cust_id")]
        public string CustId { get; set; }
        [JsonPropertyName("cust_session_id")]
        public string CustSessionId { get; set; }
        [JsonPropertyName("game_code")]
        public string GameCode { get; set; }
    }
}
