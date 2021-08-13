using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record GetBalanceResponse : ErrorResponse
    {
        [JsonPropertyName("result")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Result Data { get; set; }

        public class Result
        {
            [JsonPropertyName("user")]
            public string PlayerCode { get; set; }

            [JsonPropertyName("money")]
            public decimal Balance { get; set; }

            [JsonPropertyName("responseDate")]
            public string Timestamp { get; set; }
        }
    }
}