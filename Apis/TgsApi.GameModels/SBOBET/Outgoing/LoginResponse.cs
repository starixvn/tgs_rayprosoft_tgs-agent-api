// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Outgoing
{
    public record LoginResponse : BaseResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
