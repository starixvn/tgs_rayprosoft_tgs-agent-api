using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record KYConfiguration : PluggableConfigurationBase
    {
        public string ApiKey { get; init; }
        public Uri ApiUrl { get; init; }
    }
}
