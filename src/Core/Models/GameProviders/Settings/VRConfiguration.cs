using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.GameProviders.Settings.VR;

namespace TGS.Core.Models.GameProviders.Settings
{
    /// <summary>
    /// VR 設定檔案
    /// </summary>
    public record VRConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// Gets or sets the amount maximum.
        /// </summary>
        /// <value>
        /// The amount maximum.
        /// </value>
        public decimal AmountMaximum { get; set; }

        /// <summary>
        /// Gets or sets the amount format.
        /// </summary>
        /// <value>
        /// The amount format.
        /// </value>
        public string AmountFormat { get; set; }

        /// <summary>
        /// Gets or sets the brand merchants.
        /// </summary>
        /// <value>
        /// The brand merchants.
        /// </value>
        public VRBrandMerchant[] BrandMerchants { get; set; }
    }
}
