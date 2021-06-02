using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.SexyGaming.Common;

namespace TGS.Shared.Game.Models.SexyGaming.Outgoing
{
    public record SexyGamingGameUrlResponse : SexyGamingBaseResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
