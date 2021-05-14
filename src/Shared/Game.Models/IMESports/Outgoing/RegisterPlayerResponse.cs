// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record RegisterPlayerResponse : BaseResponse
    {
        [JsonPropertyName("Currency")]
        public string Currency { get; init; }
    }
}