// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Outgoing
{
    public record BaseRequest
    {
        [JsonPropertyName("CompanyKey")]
        public string CompanyKey { get; init; }

        [JsonPropertyName("ServerId")]
        public string ServerId { get; } = "sbo-api";
    }
}
