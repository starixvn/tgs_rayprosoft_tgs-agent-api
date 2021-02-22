// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record BaseRequest
    {
        [JsonPropertyName("CompanyKey")]
        public string CompanyKey { get; init; }

        [JsonPropertyName("ServerId")]
        public string ServerId { get; init; } = "sbo-api";
    }
}
