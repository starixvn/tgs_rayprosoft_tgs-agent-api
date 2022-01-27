using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.GameProviders.Repositories.Models;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public class EvolutionService : IEvolutionService
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IRedisPlayerLoginRecordService _playerLoginRecordService;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public EvolutionService(
            IGameProviderOperations gameProviderOperations,
            ITokenProvider tokenProvider,
            IGameActionQueries gameActionQueries,
            ISettingOperations settingOperations,
            IRedisPlayerLoginRecordService playerLoginRecordService)
        {
            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerLoginRecordService = playerLoginRecordService ?? throw new ArgumentNullException(nameof(playerLoginRecordService));
        }

        public async ValueTask<PlayerLoginRecordModel> DecryptTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }
            var loginRecordId = await _tokenProvider.DecryptAsync<Guid>(token);

            return await _playerLoginRecordService.GetAsync(loginRecordId);
        }

        public (bool RoundClosed, string ExternalTransactionId) GetCancelExternalTransactionId(
            string externalTransactionId,
            GameActionType gameActionType)
                => gameActionType == GameActionType.PlaceBet ? (true, $"{ externalTransactionId}_cancel_bet") : (false, $"{ externalTransactionId}_cancel_settle");

        public async ValueTask<EvolutionConfiguration> GetConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<EvolutionConfiguration>(SettingConsts
                .GameProviderConfigurations.Evolution);

        public async ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsAsync(
            Guid gameProviderId, string playerCode, string externalRoundId, int gameId,
            GameActionType[] gameActionTypes)
            => await _gameActionQueries
                .GetGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId, gameActionTypes)
                .ConfigureAwait(false);

        public async ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsByExternalGameActionGroupIdAsync(
                Guid gameProviderId, string playerCode, string externalGameActionGroupId)
            => await _gameActionQueries
                .GetGameActionExternalIdsByExternalGameActionGroupIdAsync(gameProviderId,
                    playerCode,
                    externalGameActionGroupId).ConfigureAwait(false);

        public async ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
            Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
        {
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId)
                .ConfigureAwait(false);

            return new()
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = autoCloseRound,
                PlatformType = platformType
            };
        }

        public async ValueTask<Guid> GetGameProviderIdAsync() => await _gameProviderOperations
            .GetGameProviderIdByCodeAsync(Consts.GameProvider.Evolution).ConfigureAwait(false);

        public void IgnoreCaseNotEqualsUserId(string userId, string playerCode)
        {
            if (userId.IgnoreCaseNotEquals(playerCode))
            {
                throw new ArgumentException();
            }
        }

        public async ValueTask<bool> NotExistBetGameActionExternalIdAsync(
                    Guid gameProviderId, string playerCode, string externalRoundId, string externalGameId,
            string externalGameActionGroupId)
        {
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId)
                .ConfigureAwait(false);
            var cachedGame = cachedGames.First();

            var datas = await _gameActionQueries.GetGameActionExternalIdsAsync(gameProviderId,
                playerCode,
                externalRoundId,
                cachedGame.Id,
                new[]
                {
                    GameActionType.PlaceBet
                }).ConfigureAwait(false);
            var data = datas.Where(x => x.ExternalGameActionGroupId == externalGameActionGroupId).SingleOrDefault();

            return data == null;
        }
    }
}
