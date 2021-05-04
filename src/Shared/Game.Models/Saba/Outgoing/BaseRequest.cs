using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record BaseRequest
    {
        [JsonPropertyName("vendor_id")]
        public string VendorId { get; init; }
    }
}