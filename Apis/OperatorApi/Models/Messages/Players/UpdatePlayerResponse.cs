using System;
using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record UpdatePlayerResponse : TgsResponse
    {
        [JsonPropertyName("updatetime")]
        public DateTimeOffset UpdateTime { get; init; }
    }
}
