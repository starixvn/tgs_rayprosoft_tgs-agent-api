using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models.Messages
{
    public class BalanceInfoResponse
    {
        [JsonPropertyName("main")]
        public decimal Main { get; init; }
    }
}
