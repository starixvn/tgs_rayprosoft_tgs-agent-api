using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.Skywind.Common;

namespace TGS.Shared.Game.Models.Skywind.Incoming
{
    public record SkywindBetRequest : SkywindSingleWalletRequest
    {
        [JsonPropertyName("cust_id")]
        public string CustId { get; set; }
        [JsonPropertyName("cust_session_id")]
        public string CustSessionId { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }
        [JsonPropertyName("game_code")]
        public string GameCode { get; set; }
        [JsonPropertyName("trx_id")]
        public string TrxId { get; set; }
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }
        [JsonPropertyName("event_type")]
        public string EventType { get; set; }
        [JsonPropertyName("event_id")]
        public string EventId { get; set; }
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
        [JsonPropertyName("game_type")]
        public string GameType { get; set; }
        [JsonPropertyName("platform")]
        public string Platform { get; set; }
        [JsonPropertyName("jp_contribution")]
        public string JpContribution { get; set; }
    }

}
