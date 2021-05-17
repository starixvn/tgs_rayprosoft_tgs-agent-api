using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BGCreatePlayerResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }
        [JsonPropertyName("userId")]
        public long UserId { get; init; }
        [JsonPropertyName("nickname")]
        public string Nickname { get; init; }
        [JsonPropertyName("regType")]
        public string RegType { get; init; }
    }
}
