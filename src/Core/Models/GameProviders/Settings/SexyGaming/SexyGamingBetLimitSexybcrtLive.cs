using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings.SexyGaming
{
    public record SexyGamingBetLimitSexybcrtLive
    {
        [JsonPropertyName("limitId")]
        public int[] LimitId { get; set; }
    }
}
