using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.GameProviders.Settings.TCG
{
    public record TCGMerchant
    {
        /// <summary>
        /// The currency code
        /// </summary>
        public string CurrencyCodev { get; init; }
        /// <summary>
        /// The merchant code
        /// </summary>
        public string MerchantCode { get; init; }
        /// <summary>
        /// The DES key
        /// </summary>
        public string DESKey { get; init; }
        /// <summary>
        /// The sh a256 key
        /// </summary>
        public string SHA256Key { get; init; }
        /// <summary>
        /// The player password
        /// </summary>
        public string PlayerPassword { get; init; }
    }
}
