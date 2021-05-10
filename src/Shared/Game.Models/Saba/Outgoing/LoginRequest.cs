using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record LoginRequest : BaseRequest
    {
        [JsonPropertyName("vendor_member_id")]
        public string PlayerCode { get; init; }
    }
}