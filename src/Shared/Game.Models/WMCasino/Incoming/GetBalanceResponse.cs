using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record GetBalanceResponse : ErrorResponse
    {
        [JsonPropertyName("result")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Result Data { get; init; }

        public class Result
        {
            [JsonPropertyName("user")]
            public string PlayerCode { get; init; }

            [JsonPropertyName("money")]
            public decimal Balance { get; init; }

            [JsonPropertyName("responseDate")]
            public string Timestamp { get; init; }
        }
    }
}