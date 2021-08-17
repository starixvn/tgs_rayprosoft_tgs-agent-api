using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record CancelWalletTransferResponse : ErrorResponse
    {
        [JsonPropertyName("result")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Result Data { get; set; }

        public class Result
        {
            [JsonPropertyName("money")]
            public decimal Amount { get; set; }

            [JsonPropertyName("cash")]
            public decimal Balance { get; set; }

            [JsonPropertyName("dealid")]
            public string DealId { get; set; }

            [JsonPropertyName("responseDate")]
            public string Timestamp { get; set; }
        }
    }
}