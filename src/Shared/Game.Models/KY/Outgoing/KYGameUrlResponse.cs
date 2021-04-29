using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.KY.Commons;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYGameUrlResponse : KYBaseDataResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
