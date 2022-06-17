using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.Avia;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;

namespace TGS.Apis.TgsApi.GameProviders.Avia.Services;

public class AviaService : IAviaService
{
    private readonly IBrandGameProviderOperations _brandGameProviderOperations;
    private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
    private readonly IGameProviderOperations _gameProviderOperations;
    private readonly IPlayerOperations _playerOperations;
    private readonly ISettingOperations _settingOperations;

    public AviaService(
        IGameProviderOperations gameProviderOperations,
        ISettingOperations settingOperations,
        IPlayerOperations playerOperations,
        IBrandGameProviderOperations brandGameProviderOperations,
        IGameProviderCurrencyCodeMapper gameProviderCurrencyCodeMapper)
    {
        _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
        _gameProviderCurrencyCodeMapper = gameProviderCurrencyCodeMapper ?? throw new ArgumentNullException(nameof(gameProviderCurrencyCodeMapper));
    }

    public async ValueTask<AviaBrandMerchant> GetBrandMerchantAsync(string brandCode)
    {
        var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<AviaConfiguration>(SettingConsts.GameProviderConfigurations.Avia);
        var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
        if (brandMerchant == null)
        {
            brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
        }
        return brandMerchant;
    }

    public async ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, Guid brandId, string language)
        => await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, language).ConfigureAwait(false);

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
        => await _gameProviderCurrencyCodeMapper.InternalToExternalAsync(currencyCode, gameProviderId).ConfigureAwait(false);

    public async Task<Guid> GetGameProviderIdAsync()
        => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.Avia).ConfigureAwait(false);

    public async Task<AviaConfiguration> GetSettingConfigurationAsync()
        => await _settingOperations.GetValueFromJsonByKeyAsync<AviaConfiguration>(SettingConsts.GameProviderConfigurations.Avia);
}
