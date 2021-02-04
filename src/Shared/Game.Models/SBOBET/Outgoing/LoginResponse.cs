// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record LoginResponse : BaseResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
