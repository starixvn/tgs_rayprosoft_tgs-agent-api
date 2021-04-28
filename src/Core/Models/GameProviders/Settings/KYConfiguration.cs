using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings
{
    /// <summary>
    /// KY 設定檔案
    /// </summary>
    public record KYConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// API接口.
        /// </summary>
        /// <value>
        /// The API URL.
        /// </value>
        public Uri ApiUrl { get; init; }
        /// <summary>
        /// 獨立拉單接口.
        /// </summary>
        /// <value>
        /// The API record URL.
        /// </value>
        public Uri ApiRecordUrl { get; init; }
        /// <summary>
        /// Gets or sets the deskey.
        /// </summary>
        /// <value>
        /// The deskey.
        /// </value>
        public string Deskey { get; init; }
        /// <summary>
        /// Gets or sets the md5key.
        /// </summary>
        /// <value>
        /// The md5key.
        /// </value>
        public string Md5key { get; init; }
        /// <summary>
        /// Gets or sets the merchant codes.
        /// </summary>
        /// <value>
        /// The merchant codes.
        /// </value>
        public Dictionary<string, string> MerchantCodes { get; init; }
    }
}
