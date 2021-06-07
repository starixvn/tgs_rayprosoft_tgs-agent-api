using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Habanero.Incoming
{
    public record QueryRequest : BaseRequest
    {
        [JsonPropertyName("queryrequest")]
        public QueryDetail Detail { get; init; }
    }

    public record QueryDetail
    {
        [JsonPropertyName("transferid")]
        public string TransactionId { get; init; }

        [JsonPropertyName("accountid")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("gameinstanceid")]
        public string RoundId { get; init; }

        [JsonPropertyName("friendlygameinstanceid")]
        public long ClientRoundId { get; init; }

        [JsonPropertyName("token")]
        public string Token { get; init; }

        [JsonPropertyName("queryamount")]
        public decimal Amount { get; init; }
    }
}