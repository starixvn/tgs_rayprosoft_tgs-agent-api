using System;
using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.GlobalApi.Models.Messages
{
    public record CreatePlayerAndLoginResponse : TgsResponse
    {
        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("loginrecordid")]
        public Guid LoginRecordId { get; init; }

        [JsonPropertyName("encryptedtoken")]
        public string EncryptedToken { get; init; }
    }
}