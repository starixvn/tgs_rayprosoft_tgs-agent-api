// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Outgoing
{
    public record BasePlayerRequest : BaseRequest
    {
        [JsonPropertyName("Username")]
        public string PlayerCode { get; init; }
    }
}
