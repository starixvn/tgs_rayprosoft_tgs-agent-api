using System.Collections.Generic;
using System.Linq;
using TGS.Core.Models.GameProviders.Settings.Jili;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services;

internal class JiliMerchantMapper : IJiliMerchantMapper
{
    private static readonly string _Default = "Default";

    public JiliMerchant GetMerchant(IEnumerable<JiliMerchant> merchants, string brandCode)
        => merchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
           merchants.FirstOrDefault(m => m.BrandCode == _Default);
}
