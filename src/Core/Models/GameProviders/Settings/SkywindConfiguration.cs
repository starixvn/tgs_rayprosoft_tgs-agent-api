using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings
{
    public record SkywindConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public string MerchId { get; init; }
        public string MerchPwd { get; init; }
    }
}
