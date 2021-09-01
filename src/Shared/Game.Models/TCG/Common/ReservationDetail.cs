using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TCG.Common
{
    public record ReservationDetail
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }
        [JsonPropertyName("ref_no")]
        public string RefNo { get; init; }
        [JsonPropertyName("username")]
        public string Username { get; init; }
        [JsonPropertyName("additionalInformation")]
        public JsonDocument AdditionalInformation { get; init; }
    }
}
