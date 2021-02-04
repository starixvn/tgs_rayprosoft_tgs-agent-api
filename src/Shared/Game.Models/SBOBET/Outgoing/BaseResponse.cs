// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.SBOBET.Enums;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record BaseResponse
    {
        [JsonPropertyName("error")]
        public ApiError Error { get; init; }

        [JsonPropertyName("serverId")]
        public string ServerId { get; init; }

        public record ApiError
        {
            [JsonPropertyName("id")]
            public ServiceApiError Code { get; init; }

            [JsonPropertyName("msg")]
            public string Message { get; init;}
        }
    }
}
