using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.JDB;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public sealed class JDBGameInitializer : IGameInitializer
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IJDBApiClient _jdbApiClient;
        private readonly JDBGameTypeMapper _jdbGameTypeMapper;

        public JDBGameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IJDBApiClient jdbApiClient,
            JDBGameTypeMapper jdbGameTypeMapper)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _jdbApiClient = jdbApiClient;
            _jdbGameTypeMapper = jdbGameTypeMapper;
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);

            var brandId = player.BrandId;
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId,
                request.Language);
            
            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<JDBConfiguration>(SettingConsts.GameProviderConfigurations.JDB);

            var getTokenRequest = new GetTokenRequest
            {
                Timestamp = DateTime.Now.Ticks,
                Parent = configuration.MerchantCode,
                UserId = player.PlayerCode,
                Language = cultureCode,
                GameType = ((int)_jdbGameTypeMapper.Get(game.Type)).ToString(),
                MachineType = game.ExternalId,
                MoreGame = configuration.MoreGame ? 1 : 0
            };

            var response = await
                _jdbApiClient.GetTokenAsync(configuration.ApiUrl, configuration.ClientDomain, getTokenRequest);

            if (response.Status == "0000")
            {
                return new() {Url = response.Path};
            }

            //TODO log error message from jdb
            return new() {Url = string.Empty};
        }
    }
}