using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record VRConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public string Deskey { get; init; }
        public string Merchantid { get; init; }
        public Dictionary<string, string> MerchantCodes { get; init; }
    }
}
