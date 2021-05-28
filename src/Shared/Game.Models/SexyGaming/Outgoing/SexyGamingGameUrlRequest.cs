using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.SexyGaming.Common;

namespace TGS.Shared.Game.Models.SexyGaming.Outgoing
{
    public record SexyGamingGameUrlRequest : SexyGamingBaseRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("isMobileLogin")]
        public bool IsMobileLogin { get; set; }
        [JsonPropertyName("platform")]
        public string Platform => "SEXYBCRT";
        [JsonPropertyName("gameType")]
        public string GameType => "LIVE";
        [JsonPropertyName("language")]
        public string Language => "en";
        [JsonPropertyName("extension1")]
        public string Extension1 { get; set; }
    }
}
