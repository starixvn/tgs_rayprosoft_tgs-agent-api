using System.Collections.Generic;
using System.Linq;
using TGS.Core.Models.GameProviders.Settings.Jili;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    internal class JiliMerchantMapper : IJiliMerchantMapper
    {
        public JiliMerchant GetMerchant(IEnumerable<JiliMerchant> merchants, string brandCode)
            => merchants.FirstOrDefault(m => m.BrandCode == brandCode);
    }
}
