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
        private readonly IJDBApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IJDBServices _services;
        private readonly ISettingOperations _settingOperations;

        public JDBGameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IJDBApiClient client,
            IJDBServices services)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _client = client;
            _services = services;
        }

        public async ValueTask<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode).ConfigureAwait(false);
            var gameProviderId = await _gameProviderOperations
                .GetGameProviderIdByCodeAsync(request.GameProviderCode)
                .ConfigureAwait(false);

            var brandId = player.BrandId;
            var cultureCode = await _brandGameProviderOperations
                .MapCultureAsync(
                    gameProviderId,
                    brandId,
                    request.Language)
                .ConfigureAwait(false);

            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId).ConfigureAwait(false);

            var configuration =
                await _settingOperations
                    .GetValueFromJsonByKeyAsync<JDBConfiguration>(
                        SettingConsts.GameProviderConfigurations.JDB)
                    .ConfigureAwait(false);

            var agent = _services.GetAgent(configuration.Agents, player.DefaultWalletCurrencyCode);
            var items = game.ExternalId.Split("_");

            if (items.Length != 2)
            {
                throw new ArgumentException($"Invalid game external id: {game.ExternalId}");
            }

            var tokenRequest = new GetTokenRequest
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Parent = agent.Id,
                UserId = player.PlayerCode,
                Language = cultureCode,
                GameType = items[0],
                MachineType = items[1],
                MoreGame = configuration.MoreGame ? 1 : 0
            };

            var response = await _client
                .GetTokenAsync(
                    configuration.ApiUrl,
                    configuration.ClientDomain,
                    configuration.AESIv,
                    configuration.AESKey,
                    tokenRequest)
                .ConfigureAwait(false);

            if (response.Status.IgnoreCaseNotEquals(GameApiStatus.Succeed.Description()))
            {
                throw new GameProviderApiErrorException(response.ErrorMessage);
            }

            return new()
            {
                Url = response.Path
            };
        }
    }
}
