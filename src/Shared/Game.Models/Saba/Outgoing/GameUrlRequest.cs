using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record GameUrlRequest : BaseRequest
    {
        [JsonPropertyName("vendor_member_id")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("platform")]
        public string Platform { get; init; }
    }
}