using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Skywind.Common;

namespace TGS.Shared.Game.Models.Skywind.Incoming
{
    public record SkywindSettleRequest : SkywindSingleWalletRequest
    {
        [Required]
        [JsonPropertyName("cust_id")]
        public string CustId { get; set; }
        [Required]
        [JsonPropertyName("cust_session_id")]
        public string CustSessionId { get; set; }
        [Required]
        [JsonPropertyName("round_id")]
        public string RoundId { get; set; }
        [Required]
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [Required]
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }
        [Required]
        [JsonPropertyName("game_code")]
        public string GameCode { get; set; }
        [Required]
        [JsonPropertyName("trx_id")]
        public string TrxId { get; set; }
        [Required]
        [JsonPropertyName("game_id")]
        public long GameId { get; set; }
        [Required]
        [JsonPropertyName("event_type")]
        public string EventType { get; set; }
        [Required]
        [JsonPropertyName("event_id")]
        public int EventId { get; set; }
        [Required]
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
        [Required]
        [JsonPropertyName("game_type")]
        public string GameType { get; set; }
        [Required]
        [JsonPropertyName("game_status")]
        public string GameStatus { get; set; }
        [Required]
        [JsonPropertyName("platform")]
        public string Platform { get; set; }
        [JsonPropertyName("jp_ids")]
        public string[] JpIds { get; set; }
        [JsonPropertyName("promo_id ")]
        public string PromoId { get; set; }
    }
}
