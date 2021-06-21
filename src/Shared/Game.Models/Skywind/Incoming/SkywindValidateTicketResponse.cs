using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Skywind.Incoming
{
    public record SkywindValidateTicketResponse
    {
        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }
        [JsonPropertyName("error_msg")]
        public string ErrorMsg { get; set; }
        [JsonPropertyName("cust_id")]
        public string CustId { get; set; }
        [JsonPropertyName("cust_session_id")]
        public string CustSessionId { get; set; }
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }
        [JsonPropertyName("test_cust")]
        public bool TestCust { get; set; }
        [JsonPropertyName("country")]
        public string Country { get; set; }
       /*可以不用帶入
        [JsonPropertyName("game_group")]
        public string GameGroup { get; set; }
        [JsonPropertyName("rci")]
        public int Rci { get; set; }
        [JsonPropertyName("rce")]
        public int Rce { get; set; }
       */
    }
}