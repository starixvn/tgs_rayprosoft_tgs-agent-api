using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.GameProviders.Settings.KY;

namespace TGS.Core.Models.GameProviders.Settings
{
    /// <summary>
    /// KY 設定檔案
    /// </summary>
    public record KYConfiguration : PluggableConfigurationBase
    {

        /// <summary>
        /// Gets or sets the brand merchants.
        /// </summary>
        /// <value>
        /// The brand merchants.
        /// </value>
        public KYBrandMerchant[] BrandMerchants { get; set; }
    }

}
