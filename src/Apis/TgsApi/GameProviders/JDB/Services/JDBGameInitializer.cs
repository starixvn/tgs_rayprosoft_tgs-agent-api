using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.JDB;
using TGS.Shared.Game.Models.JDB.Enums;
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
        private readonly IJDBServices _jdbServices;

        public JDBGameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IJDBApiClient jdbApiClient,
            JDBGameTypeMapper jdbGameTypeMapper,
            IJDBServices jdbServices)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _jdbApiClient = jdbApiClient;
            _jdbGameTypeMapper = jdbGameTypeMapper;
            _jdbServices = jdbServices;
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
            
            var agent = _jdbServices.GetAgent(configuration, player.DefaultWalletCurrencyCode);

            var response = await _jdbApiClient.GetTokenAsync(new GetTokenRequest
            {
                Timestamp = DateTime.Now.Ticks,
                Parent = agent.Id,
                UserId = player.PlayerCode,
                Language = cultureCode,
                GameType = ((int)_jdbGameTypeMapper.Get(game.Type)).ToString(),
                MachineType = game.ExternalId,
                MoreGame = configuration.MoreGame ? 1 : 0
            });

            if (response.Status.NotEquals(GameApiStatus.Succeed.Description()))
            {
                throw new GameProviderApiErrorException(response.ErrorMessage);
            }
            
            return new() {Url = response.Path};
        }
    }
}