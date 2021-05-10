using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Commons
{
    public record KYApiResponse<TData>
    {
        [JsonPropertyName("m")]
        public string Message { get; init; }

        [JsonPropertyName("s")]
        public int SubMethod { get; init; }

        [JsonPropertyName("d")]
        public TData Data { get; init; }
    }
}
