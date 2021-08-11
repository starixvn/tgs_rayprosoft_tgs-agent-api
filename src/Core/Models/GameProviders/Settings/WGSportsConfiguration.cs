using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings
{
    /// <summary>
    /// WG Sports 設定檔案
    /// </summary>
    public record WGSportsConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }

        public string PartnerCode { get; init; }

        public string PartnerKey { get; init; }

        public string IV { get; init; }
    }
}
