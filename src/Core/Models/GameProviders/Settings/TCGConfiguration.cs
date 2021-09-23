using System;
using TGS.Core.Models.GameProviders.Settings.TCG;

namespace TGS.Core.Models.GameProviders.Settings
{
    /// <summary>
    /// TCG 設定檔案
    /// </summary>
    public record TCGConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// The API URL
        /// </summary>
        public Uri ApiUrl { get; init; }
        /// <summary>
        /// The merchants
        /// </summary>
        public TCGMerchant[] Merchants { get; init; }
    }
}
