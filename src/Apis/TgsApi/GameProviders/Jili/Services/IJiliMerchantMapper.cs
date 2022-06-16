using System.Collections.Generic;
using TGS.Core.Models.GameProviders.Settings.Jili;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services;

public interface IJiliMerchantMapper
{
    JiliMerchant GetMerchant(IEnumerable<JiliMerchant> merchants, string brandCode);
}