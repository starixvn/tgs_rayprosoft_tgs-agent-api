using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Incoming
{
    public record KYGameUrlResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; init; }

        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
