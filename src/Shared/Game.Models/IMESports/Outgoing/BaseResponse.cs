// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.IMESports.Enums;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record BaseResponse
    {
        [JsonPropertyName("Code")]
        public GameApiError ErrorCode { get; init; }

        [JsonPropertyName("Message")]
        public string Message { get; init; }
    }
}
