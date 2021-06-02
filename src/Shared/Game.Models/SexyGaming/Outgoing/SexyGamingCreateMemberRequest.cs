using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.SexyGaming.Common;

namespace TGS.Shared.Game.Models.SexyGaming.Outgoing
{
    public record SexyGamingCreateMemberRequest : SexyGamingBaseRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
        [JsonPropertyName("betLimit")]
        public string BetLimit { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; }
    }
}
