using System;
using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record LoginResponse : TgsResponse
    {
        [JsonPropertyName("token")]
        public string PlayerToken {  get; init; }

        [JsonPropertyName("logintime")]
        public DateTimeOffset LoginTime { get; init; }
    }
}
