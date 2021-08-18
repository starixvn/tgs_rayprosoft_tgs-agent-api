using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.ScheduleConfig.WGHorse;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.WGHorse.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.WGHorse
{
    public interface IWGHorseGameProviderService
    {
        /// <summary>
        /// Gets the setting configuration.
        /// </summary>
        /// <returns></returns>
        ValueTask<WGHorseConfiguration> GetSettingConfiguration();

        /// <summary>
        /// Gets the game provider identifier.
        /// </summary>
        /// <returns></returns>
        ValueTask<Guid> GetGameProviderId();

        /// <summary>
        /// Gets the game provider schedule.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GetGameProviderSchedule(Guid gameProviderId);

        /// <summary>
        /// Games the provider schedule initialize.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleInit(Guid gameProviderId);

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="responses">The responses.</param>
        /// <param name="sysTime">The system time.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, WGHorseGPGameRecordResponse responses, DateTimeOffset sysTime);

        /// <summary>
        /// Gets the game provider schedule configuration.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <returns></returns>
        WGHorseGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);
    }
    public class WGHorseGameProviderService : IWGHorseGameProviderService
    {
        private readonly IJsonSerializationProvider _json;
        private readonly ISettingOperations _settingOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;

        public WGHorseGameProviderService(
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
        async ValueTask<WGHorseConfiguration> IWGHorseGameProviderService.GetSettingConfiguration()
            => await _settingOperations.GetValueFromJsonByKeyAsync<WGHorseConfiguration>(SettingConsts.GameProviderConfigurations.WGHorse).ConfigureAwait(false);

        /// <summary>
        /// Gets the game provider identifier.
        /// </summary>
        /// <returns></returns>
        async ValueTask<Guid> IWGHorseGameProviderService.GetGameProviderId()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderConst.WGHorse).ConfigureAwait(false);

        /// <summary>
        /// Gets the game provider schedule.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        public async ValueTask<GameProviderSchedule> GetGameProviderSchedule(Guid gameProviderId)
        => await _gameProviderScheduleRepository.GetAsync(gameProviderId, string.Empty, string.Empty, string.Empty).ConfigureAwait(false);

        /// <summary>
        /// Games the provider schedule initialize.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        async ValueTask<GameProviderSchedule> IWGHorseGameProviderService.GameProviderScheduleInit(Guid gameProviderId)
        {
            var intervalDayMax = 60;
            var intervalDay = 30;
            var sysTime = DateTimeOffset.UtcNow;
            var config = new WGHorseGameProviderScheduleConfig()
            {
                IntervalDayMax = intervalDayMax,
                IntervalDay = intervalDay,
                LastGameRecordTime = new DateTimeOffset(Convert.ToDateTime("1970-01-01"), TimeSpan.FromHours(+0)),
                StartTime = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-dd")).AddDays(intervalDay * -1),
                EndTime = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-dd")),
            };

            var gameProviderSchedule = new GameProviderSchedule()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProviderId,
                GameProviderCode = GameProviderConst.WGHorse,
                BrandCode = string.Empty,
                AgentId = string.Empty,
                CurrencyCode = string.Empty,
                Config = _json.SerializeToString(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
            };
            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            return await GetGameProviderSchedule(gameProviderId).ConfigureAwait(false); ;
        }

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="responses">The responses.</param>
        /// <param name="sysTime">The system time.</param>
        /// <returns></returns>
        async ValueTask<GameProviderSchedule> IWGHorseGameProviderService.GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, WGHorseGPGameRecordResponse responses, DateTimeOffset sysTime)
        {
            var config = _json.DeserializeFromString<WGHorseGameProviderScheduleConfig>(gameProviderSchedule.Config);

            if (config.EndTime > sysTime || (sysTime - config.EndTime).TotalDays < (double)config.IntervalDay)
            {
                config.StartTime = sysTime.AddDays((double)config.IntervalDay * -1);
                config.EndTime = sysTime;
            }
            else
            {
                var endTime = config.EndTime.AddDays(1);
                config.StartTime = config.EndTime;
                config.EndTime = (endTime > sysTime) ? sysTime : endTime;
            }

            config.LastGameRecordTime = (responses.LastGameRecorTime.HasValue) ? responses.LastGameRecorTime.Value : config.LastGameRecordTime;
            gameProviderSchedule.Config = _json.SerializeToString(config);
            gameProviderSchedule.UpdatedOn = DateTimeOffset.UtcNow;

            var updateResult = await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);
            if (updateResult.ModifiedCount > 0)
            {
                return await GetGameProviderSchedule(gameProviderSchedule.GameProviderId).ConfigureAwait(false); ;
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
        WGHorseGameProviderScheduleConfig IWGHorseGameProviderService.GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
        => _json.DeserializeFromString<WGHorseGameProviderScheduleConfig>(gameProviderSchedule.Config);
    }
}
