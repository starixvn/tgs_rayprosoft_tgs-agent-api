using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record GetRoundHistoryRequest : BaseRequest
    {
        [JsonPropertyName("trans_id")]
        public string ExternalTransactionId { get; init; }
    }
}