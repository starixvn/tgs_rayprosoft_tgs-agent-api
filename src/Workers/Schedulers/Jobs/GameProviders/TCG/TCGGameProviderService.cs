using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.ScheduleConfig.TCG;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.TCG.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.TCG
{
    public interface ITCGGameProviderService
    {
        /// <summary>
        /// Gets the setting configuration.
        /// </summary>
        /// <returns></returns>
        Task<TCGConfiguration> GetSettingConfigurationAsync();

        /// <summary>
        /// Gets the game provider identifier.
        /// </summary>
        /// <returns></returns>
        Task<Guid> GetGameProviderIdAsync();

        /// <summary>
        /// Gets the game provider code.
        /// </summary>
        /// <returns></returns>
        string GetGameProviderCode();

        /// <summary>
        /// Gets the game provider schedule asynchronous.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="subGP">The sub gp.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <param name="merchantCode">The merchant code.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string subGP, string currencyCode, string merchantCode);

        /// <summary>
        /// Gets the game provider schedule asynchronous.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <param name="agentId">The agent identifier.</param>
        /// <returns></returns>
        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string currencyCode, string agentId);

        /// <summary>
        /// Games the provider schedule initialize asynchronous.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="subGP">The sub gp.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <param name="merchantCode">The merchant code.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string subGP, string currencyCode, string merchantCode);

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="responses">The responses.</param>
        /// <param name="sysTime">The system time.</param>
        /// <returns></returns>
        ValueTask GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, TCGGPGameRecordResponse responses, DateTimeOffset sysTime);

        /// <summary>
        /// Gets the game provider schedule configuration.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <returns></returns>
        TCGGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);

        /// <summary>
        /// Gets the game lottery lobby.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        Task<CachedGame[]> GetGameLotteryLobbyAsync(Guid gameProviderId);

        /// <summary>
        /// Gets the sub gp and merchant code.
        /// </summary>
        /// <param name="agentId">The agent identifier.</param>
        /// <returns></returns>
        (string SubGP, string MerchantCode) GetSubGPAndMerchantCode(string agentId);
    }
    public class TCGGameProviderService : ITCGGameProviderService
    {
        private readonly IJsonSerializationProvider _json;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISettingOperations _settingOperations;

        public TCGGameProviderService(
            IJsonSerializationProvider json,
            IGameProviderScheduleRepository gameProviderScheduleRepository,
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations)
        {
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _gameProviderScheduleRepository = gameProviderScheduleRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleRepository));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        }

        public async ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string subGP, string currencyCode, string merchantCode)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var config = new TCGGameProviderScheduleConfig()
            {
                LastGameRecorDirectory = string.Empty,
                LastGameRecorFile = string.Empty,
            };

            var gameProviderSchedule = new GameProviderSchedule()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProviderId,
                GameProviderCode = GetGameProviderCode(),
                BrandCode = string.Empty,
                AgentId = GetAgentId(subGP, merchantCode),
                CurrencyCode = currencyCode,
                Config = _json.SerializeToString(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
            };
            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            return gameProviderSchedule;
        }

        public async ValueTask GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, TCGGPGameRecordResponse responses, DateTimeOffset sysTime)
        {
            var config = GetGameProviderScheduleConfig(gameProviderSchedule);
            if (!string.IsNullOrEmpty(responses.LastGameRecorDirectory) && !string.IsNullOrEmpty(responses.LastGameRecorFile))
            {
                config.LastGameRecorDirectory = responses.LastGameRecorDirectory;
                config.LastGameRecorFile = responses.LastGameRecorFile;
            }
            gameProviderSchedule.Config = _json.SerializeToString(config);
            gameProviderSchedule.UpdatedOn = DateTimeOffset.UtcNow;

            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);
        }

        public string GetGameProviderCode()
            => GameProviderConst.TCG;

        public Task<Guid> GetGameProviderIdAsync()
            => _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderConst.TCG);

        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string subGP, string currencyCode, string merchantCode)
         => _gameProviderScheduleRepository.GetAsync(gameProviderId, string.Empty, GetAgentId(subGP, merchantCode), currencyCode);

        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string currencyCode, string agentId)
            => _gameProviderScheduleRepository.GetAsync(gameProviderId, string.Empty, agentId, currencyCode);

        public TCGGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
            => _json.DeserializeFromString<TCGGameProviderScheduleConfig>(gameProviderSchedule.Config);

        public Task<TCGConfiguration> GetSettingConfigurationAsync()
            => _settingOperations.GetValueFromJsonByKeyAsync<TCGConfiguration>(SettingConsts.GameProviderConfigurations.TCG);

        public async Task<CachedGame[]> GetGameLotteryLobbyAsync(Guid gameProviderId)
        {
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId).ConfigureAwait(false);
            return games.Where(x => x.Type == GameType.LotteryLobby).ToArray();
        }

        public (string SubGP, string MerchantCode) GetSubGPAndMerchantCode(string agentId)
        {
            var splits = agentId.Split("-");
            return (splits[0], splits[1]);
        }

        private string GetAgentId(string subGP, string merchantCode)
            => string.Format("{0}-{1}", subGP, merchantCode);
    }
}
