using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.ScheduleConfig.KY;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.KY.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.KY
{
    public interface IKYGameProviderService
    {
        /// <summary>
        /// Gets the setting configuration.
        /// </summary>
        /// <returns></returns>
        ValueTask<KYConfiguration> GetSettingConfigurationAsync();

        /// <summary>
        /// Gets the game provider identifier.
        /// </summary>
        /// <returns></returns>
        ValueTask<Guid> GetGameProviderIdAsync();

        /// <summary>
        /// Gets the game provider schedule.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="agentId">The agent identifier.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode);

        /// <summary>
        /// Games the provider schedule initialize.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="agentId">The agent identifier.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode);

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="responses">The responses.</param>
        /// <param name="sysTime">The system time.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, KYGPGameRecordResponse responses, DateTimeOffset sysTime);

        /// <summary>
        /// Gets the game provider schedule configuration.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <returns></returns>
        KYGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);
    }
    public sealed class KYGameProviderService : IKYGameProviderService
    {
        private readonly IJsonSerializationProvider _json;
        private readonly ISettingOperations _settingOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;

        public KYGameProviderService(
            IJsonSerializationProvider json,
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderScheduleRepository gameProviderScheduleRepository
           )
        {
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameProviderScheduleRepository = gameProviderScheduleRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleRepository));
        }

        /// <summary>
        /// Gets the setting configuration.
        /// </summary>
        /// <returns></returns>
        async ValueTask<KYConfiguration> IKYGameProviderService.GetSettingConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts.GameProviderConfigurations.KY).ConfigureAwait(false);

        /// <summary>
        /// Gets the game provider identifier.
        /// </summary>
        /// <returns></returns>
        async ValueTask<Guid> IKYGameProviderService.GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderConst.KY).ConfigureAwait(false);

        /// <summary>
        /// Gets the game provider schedule.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="agentId">The agent identifier.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns></returns>
        public async ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode)
        => await _gameProviderScheduleRepository.GetAsync(gameProviderId, brandCode, agentId, currencyCode).ConfigureAwait(false);

        /// <summary>
        /// Games the provider schedule initialize.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        async ValueTask<GameProviderSchedule> IKYGameProviderService.GameProviderScheduleInitAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var intervalMinuteMax = 40m;
            var intervalMinute = 10m;
            var config = new KYGameProviderScheduleConfig()
            {
                IntervalMinuteMax = intervalMinuteMax,
                IntervalMinute = intervalMinute,
                LastGameRecordTime = new DateTimeOffset(Convert.ToDateTime("1970-01-01"), TimeSpan.FromHours(+8)),
                StartTime = sysTime.AddSeconds((double)intervalMinute * 60 * -1),
                EndTime = sysTime
            };

            var gameProviderSchedule = new GameProviderSchedule()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProviderId,
                GameProviderCode = GameProviderConst.KY,
                BrandCode = brandCode,
                AgentId = agentId,
                CurrencyCode = currencyCode,
                Config = _json.SerializeToString(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
            };
            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            return gameProviderSchedule;
        }

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="responses">The responses.</param>
        /// <param name="sysTime">The system time.</param>
        /// <returns></returns>
        async ValueTask<GameProviderSchedule> IKYGameProviderService.GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, KYGPGameRecordResponse responses, DateTimeOffset sysTime)
        {
            var config = _json.DeserializeFromString<KYGameProviderScheduleConfig>(gameProviderSchedule.Config);

            if (config.EndTime > sysTime || (sysTime - config.EndTime).TotalSeconds < (double)config.IntervalMinute * 60)
            {
                config.StartTime = sysTime.AddSeconds((double)config.IntervalMinute * 60 * -1);
                config.EndTime = sysTime;
            }
            else
            {
                var endTime = config.EndTime.AddSeconds((double)config.IntervalMinute * 60);
                config.StartTime = config.EndTime;
                config.EndTime = (endTime > sysTime) ? sysTime : endTime;
            }

            config.LastGameRecordTime = (responses.LastGameRecorTime.HasValue) ? responses.LastGameRecorTime.Value : config.LastGameRecordTime;
            gameProviderSchedule.Config = _json.SerializeToString(config);
            gameProviderSchedule.UpdatedOn = DateTimeOffset.UtcNow;

            var updateResult = await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);
            if (updateResult.ModifiedCount > 0)
            {
                return await GetGameProviderScheduleAsync(gameProviderSchedule.GameProviderId, gameProviderSchedule.BrandCode, gameProviderSchedule.AgentId, gameProviderSchedule.CurrencyCode).ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the game provider schedule configuration.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <returns></returns>
        KYGameProviderScheduleConfig IKYGameProviderService.GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
        => _json.DeserializeFromString<KYGameProviderScheduleConfig>(gameProviderSchedule.Config);
    }
}
