using System;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.GameProviders.Repositories.Models;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public class SkywindService : ISkywindService
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IRedisPlayerLoginRecordService _playerLoginRecordService;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public SkywindService(
            IGameProviderOperations gameProviderOperations,
            ITokenProvider tokenProvider,
            IPlayerOperations playerOperations,
            IGameActionQueries gameActionQueries,
            ISettingOperations settingOperations,
            IBrandOperations brandOperations,
            IRedisPlayerLoginRecordService playerLoginRecordService)
        {
            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
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

        public async ValueTask<CachedBrandGameProviderConfiguration> GetBrandGameProviderConfigurationAsync(
            Guid brandId, Guid gameProviderId)
            => await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

        public async ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode)
            => await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);

        public async ValueTask<SkywindConfiguration> GetConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<SkywindConfiguration>(SettingConsts
                .GameProviderConfigurations.Skywind);

        public string GetExternalRoundId(string roundId, long eventId) => $"{roundId}_{eventId}";

        public string GetExternalTransactionId(string trxId, long eventId, string action)
        {
            var externalTransaction = $"{trxId}_{eventId}";

            return string.IsNullOrEmpty(action) ? externalTransaction : $"{externalTransaction}_{action}";
        }

        public async ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(
            Guid gameProviderId,
            string playerCode,
            string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(
                gameProviderId,
                playerCode,
                externalTransactionId)
                .ConfigureAwait(false);

        public async ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
            Guid gameProviderId,
            Guid gameProviderConfigurationId,
            PlatformType platformType)
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

        public async ValueTask<Guid> GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.Skywind).ConfigureAwait(false);

        public bool IsTest(string playerCode)
            => playerCode.IgnoreCaseEquals("4uedhjjznnw4") || playerCode.IgnoreCaseEquals("4ueua6d0au9x");
    }
}
