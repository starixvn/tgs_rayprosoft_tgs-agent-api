using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings.SexyGaming
{
    public record SexyGamingBetLimit
    {
        public Dictionary<int, Guid> SexybcrtMapper { get; set; }
    }
}
