using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.ScheduleConfig.WGSports;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.WGSports.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.WGSports
{
    public interface IWGSportsGameProviderService
    {
        /// <summary>
        /// Gets the setting configuration.
        /// </summary>
        /// <returns></returns>
        ValueTask<WGSportsConfiguration> GetSettingConfigurationAsync();

        /// <summary>
        /// Gets the game provider identifier.
        /// </summary>
        /// <returns></returns>
        ValueTask<Guid> GetGameProviderIdAsync();

        /// <summary>
        /// Gets the game provider schedule.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId);

        /// <summary>
        /// Games the provider schedule initialize.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId);

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="responses">The responses.</param>
        /// <param name="sysTime">The system time.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, WGSportsGPGameRecordResponse responses, DateTimeOffset sysTime);

        /// <summary>
        /// Gets the game provider schedule configuration.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <returns></returns>
        WGSportsGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);

    }
    public sealed class WGSportsGameProviderService : IWGSportsGameProviderService
    {
        private readonly IJsonSerializationProvider _json;
        private readonly ISettingOperations _settingOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;

        public WGSportsGameProviderService(
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
        async ValueTask<WGSportsConfiguration> IWGSportsGameProviderService.GetSettingConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<WGSportsConfiguration>(SettingConsts.GameProviderConfigurations.WGSports).ConfigureAwait(false);

        /// <summary>
        /// Gets the game provider identifier.
        /// </summary>
        /// <returns></returns>
        async ValueTask<Guid> IWGSportsGameProviderService.GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderConst.WGSports).ConfigureAwait(false);

        /// <summary>
        /// Gets the game provider schedule.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        public async ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId)
        => await _gameProviderScheduleRepository.GetAsync(gameProviderId, string.Empty, string.Empty, string.Empty).ConfigureAwait(false);

        /// <summary>
        /// Games the provider schedule initialize.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        async ValueTask<GameProviderSchedule> IWGSportsGameProviderService.GameProviderScheduleInitAsync(Guid gameProviderId)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var intervalMinuteMax = 24 * 60m;
            var intervalMinute = 20m;
            var config = new WGSportsGameProviderScheduleConfig()
            {
                IntervalMinuteMax = intervalMinuteMax,
                IntervalMinute = intervalMinute,
                LastGameRecordTime = new DateTimeOffset(Convert.ToDateTime("1970-01-01"), TimeSpan.FromHours(+0)),
                StartTime = sysTime.AddSeconds((double)intervalMinute * 60 * -1),
                EndTime = sysTime
            };

            var gameProviderSchedule = new GameProviderSchedule()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProviderId,
                GameProviderCode = GameProviderConst.WGSports,
                BrandCode = string.Empty,
                AgentId = string.Empty,
                CurrencyCode = string.Empty,
                Config = _json.SerializeToString(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
            };
            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            return await GetGameProviderScheduleAsync(gameProviderId).ConfigureAwait(false); ;
        }

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="responses">The responses.</param>
        /// <param name="sysTime">The system time.</param>
        /// <returns></returns>
        async ValueTask<GameProviderSchedule> IWGSportsGameProviderService.GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, WGSportsGPGameRecordResponse responses, DateTimeOffset sysTime)
        {
            var config = _json.DeserializeFromString<WGSportsGameProviderScheduleConfig>(gameProviderSchedule.Config);

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
                return await GetGameProviderScheduleAsync(gameProviderSchedule.GameProviderId).ConfigureAwait(false);
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
        WGSportsGameProviderScheduleConfig IWGSportsGameProviderService.GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
        => _json.DeserializeFromString<WGSportsGameProviderScheduleConfig>(gameProviderSchedule.Config);
    }
}
