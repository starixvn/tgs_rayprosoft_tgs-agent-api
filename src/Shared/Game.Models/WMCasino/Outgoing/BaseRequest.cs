using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Outgoing
{
    public abstract record BaseRequest
    {
        [JsonPropertyName("cmd")]
        public abstract string Action { get; }

        [JsonPropertyName("vendorId")]
        public string VendorId { get; init; }

        [JsonPropertyName("signature")]
        public string Signature { get; init; }
    }
}