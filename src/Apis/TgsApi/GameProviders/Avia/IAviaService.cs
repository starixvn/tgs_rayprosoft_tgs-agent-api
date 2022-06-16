using System;
using System.Threading.Tasks;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.Avia;

namespace TGS.Apis.TgsApi.GameProviders.Avia;

public interface IAviaService
{
    ValueTask<AviaBrandMerchant> GetBrandMerchantAsync(string brandCode);

    ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language);

    ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

    Task<Guid> GetGameProviderIdAsync();

    Task<AviaConfiguration> GetSettingConfigurationAsync();
}
