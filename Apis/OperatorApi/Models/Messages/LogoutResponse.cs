using System;
using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record LogoutResponse : TgsResponse
    {
        [JsonPropertyName("updatetime")]
        public DateTimeOffset UpdateTime { get; init; }
    }
}
