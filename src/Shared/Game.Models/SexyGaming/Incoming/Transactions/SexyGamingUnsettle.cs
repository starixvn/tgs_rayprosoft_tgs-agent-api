using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.SexyGaming.Enums;

namespace TGS.Shared.Game.Models.SexyGaming.Incoming
{
    public record SexyGamingUnsettle
    {
        [JsonPropertyName("platformTxId")]
        public string PlatformTxId { get; init; }
        [JsonPropertyName("userId")]
        public string UserId { get; init; }
        [JsonPropertyName("platform")]
        public string Platform { get; init; }
        [JsonPropertyName("gameType")]
        public string GameType { get; init; }
        [JsonPropertyName("gameCode")]
        public string GameCode { get; init; }
        [JsonPropertyName("updateTime")]
        public DateTimeOffset UpdateTime { get; init; }
        [JsonPropertyName("voidType")]
        public int VoidType { get; init; }
    }
}
