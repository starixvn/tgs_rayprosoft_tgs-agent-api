// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record BasePlayerRequest : BaseRequest
    {
        [JsonPropertyName("PlayerId")]
        public string PlayerCode { get; init; }
    }
}
