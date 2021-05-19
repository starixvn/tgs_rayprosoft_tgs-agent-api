// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record BaseRequest
    {
        [JsonPropertyName("MerchantCode")]
        public string MerchantCode { get; init; }
    }
}
