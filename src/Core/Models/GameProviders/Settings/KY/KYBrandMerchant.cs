using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings.KY
{
    /// <summary>
    /// KY Brand 商戶
    /// </summary>
    public record KYBrandMerchant
    {
        /// <summary>
        /// Gets or sets the brand code.
        /// </summary>
        /// <value>
        /// The brand code.
        /// </value>
        public string BrandCode { get; init; }
        /// <summary>
        /// API接口.
        /// </summary>
        /// <value>
        /// The API URL.
        /// </value>
        public Uri ApiUrl { get; set; }
        /// <summary>
        /// 獨立拉單接口.
        /// </summary>
        /// <value>
        /// The API record URL.
        /// </value>
        public Uri ApiRecordUrl { get; set; }
        /// <summary>
        /// Gets or sets the deskey.
        /// </summary>
        /// <value>
        /// The deskey.
        /// </value>
        public string Deskey { get; set; }
        /// <summary>
        /// Gets or sets the md5key.
        /// </summary>
        /// <value>
        /// The md5key.
        /// </value>
        public string Md5key { get; set; }
        /// <summary>
        /// Gets or sets the merchant codes.
        /// </summary>
        /// <value>
        /// The merchant codes.
        /// </value>
        public KYAgentMapper[] AgentMappers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KYBrandMerchant"/> is status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if status; otherwise, <c>false</c>.
        /// </value>
        public bool Status { get; set; }
    }
}
