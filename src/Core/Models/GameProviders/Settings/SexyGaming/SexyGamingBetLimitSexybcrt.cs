using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings.SexyGaming
{
    public record SexyGamingBetLimitSexybcrt
    {
        [JsonPropertyName("LIVE")]
        public SexyGamingBetLimitSexybcrtLive Live { get; set; }
    }
}
