using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings
{
    /// <summary>
    /// WG Horse 設定檔案
    /// </summary>
    public record WGHorseConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// The amount maximum
        /// </summary>
        public decimal AmountMaximum { get; init; }

        /// <summary>
        /// The amount format
        /// </summary>
        public string AmountFormat { get; init; }

        public Uri ApiUrl { get; init; }

        public string PartnerCode { get; init; }

        public string PartnerKey { get; init; }

        public string IV { get; init; }
    }
}
