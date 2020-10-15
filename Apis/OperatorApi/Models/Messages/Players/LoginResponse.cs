using System;
using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record LoginResponse : TgsResponse
    {
        [JsonPropertyName("playercode")]
        public string PlayerCode {  get; init; }

        [JsonPropertyName("logintime")]
        public DateTimeOffset LoginTime { get; init; }
    }
}
