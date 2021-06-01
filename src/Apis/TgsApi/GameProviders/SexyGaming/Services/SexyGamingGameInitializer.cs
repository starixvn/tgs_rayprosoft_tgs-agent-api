using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.SexyGaming;
using TGS.Shared.Game.Models.SexyGaming.Outgoing;
using TGS.Shared.Game.Models.SexyGaming.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations.CodeMappers;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public sealed class SexyGamingGameInitializer : IGameInitializer
    {

        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly ISexyGamingApiClient _client;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISexyGamingServices _sexyGamingServices;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ISettingOperations _settingOperations;
        private readonly IGameProviderCurrencyCodeMapper _currencyCodeMapper;

        public SexyGamingGameInitializer(
            IBrandGameProviderOperations brandGameProviderOperations,
            ISexyGamingApiClient client,
            IPlayerOperations playerOperations,
            ISexyGamingServices sexyGamingServices,
            IJsonSerializationProvider jsonSerializationProvider,
            ISettingOperations settingOperations,
            IGameProviderCurrencyCodeMapper currencyCodeMapper
            )
        {
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _sexyGamingServices = sexyGamingServices ?? throw new ArgumentNullException(nameof(sexyGamingServices));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _currencyCodeMapper = currencyCodeMapper ?? throw new ArgumentNullException(nameof(currencyCodeMapper));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {

            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var gameProviderId = await _sexyGamingServices.GetGameProviderId().ConfigureAwait(false);
            var brandId = player.BrandId;
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, request.Language);
            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<SexyGamingConfiguration>(SettingConsts.GameProviderConfigurations.SexyGaming);
            var data = new SexyGamingGameUrlRequest()
            {
                UserId = player.PlayerCode,
                IsMobileLogin = request.Token.PlatformType == Core.Models.Enums.PlatformType.Mobile,
                Language = cultureCode,
                Extension1 = _jsonSerializationProvider.SerializeToString(new { token = request.Token.LoginRecordId }),
            };

            var url = default(string);
            var responseGameUrl = await _client.GetGameUrlAsync(configuration, data).ConfigureAwait(false);
            if (responseGameUrl.Status.IgnoreCaseNotEquals(((int)GameApiError.Success).ToString("d4")))
            {

                if (responseGameUrl.Status.IgnoreCaseEquals(((int)GameApiError.AccountIsNotExists).ToString("d4")))
                {

                    var BetLime = configuration.BetLimit.SexybcrtMapper.Where(x => x.Value == player.DefaultWalletCurrencyCode).Select(x => x.Key).ToArray();
                    var currencyCode = await _currencyCodeMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);

                    var createMemberRequest = new SexyGamingCreateMemberRequest()
                    {
                        UserId = player.PlayerCode,
                        Currency = currencyCode,
                        BetLimit = _jsonSerializationProvider.SerializeToString(new
                        {
                            SEXYBCRT = new
                            {
                                LIVE = new { limitId = BetLime }
                            }
                        }),
                        Language = cultureCode
                    };
                    await _client.CreatePlayerAsync(configuration, createMemberRequest).ConfigureAwait(false);

                    data = new SexyGamingGameUrlRequest()
                    {
                        UserId = player.PlayerCode,
                        IsMobileLogin = request.Token.PlatformType == Core.Models.Enums.PlatformType.Mobile,
                        Language = cultureCode,
                        Extension1 = _jsonSerializationProvider.SerializeToString(new { token = request.Token.LoginRecordId }),
                    };

                    responseGameUrl = await _client.GetGameUrlAsync(configuration, data).ConfigureAwait(false);
                    if (responseGameUrl.Status.IgnoreCaseEquals(((int)GameApiError.AccountIsNotExists).ToString("d4")))
                    {
                        url = responseGameUrl.Url;
                    }
                    else
                    {
                        throw new GameProviderApiErrorException(responseGameUrl.Desc);
                    }
                }
                else
                {
                    throw new GameProviderApiErrorException(responseGameUrl.Desc);
                }

            }
            else
            {
                url = responseGameUrl.Url;
            }
            return new() { Url = url };
        }
    }
}