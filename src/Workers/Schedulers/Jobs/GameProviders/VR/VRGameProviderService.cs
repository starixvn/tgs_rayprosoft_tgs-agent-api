using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.ScheduleConfig.VR;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.VR.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.VR
{
    public interface IVRGameProviderService
    {
        /// <summary>
        /// Gets the setting configuration.
        /// </summary>
        /// <returns></returns>
        ValueTask<VRConfiguration> GetSettingConfigurationAsync();

        /// <summary>
        /// Gets the game provider identifier.
        /// </summary>
        /// <returns></returns>
        ValueTask<Guid> GetGameProviderIdAsync();

        /// <summary>
        /// Gets the game provider code.
        /// </summary>
        /// <returns></returns>
        string GetGameProviderCode();

        /// <summary>
        /// Gets the game provider schedule asynchronous.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="merchantid">The merchantid.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode, string merchantid);

        /// <summary>
        /// Games the provider schedule initialize asynchronous.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="merchantid">The merchantid.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string brandCode, string merchantid);

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="responses">The responses.</param>
        /// <param name="sysTime">The system time.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, VRGPGameRecordResponse responses, DateTimeOffset sysTime);

        /// <summary>
        /// Gets the game provider schedule configuration.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <returns></returns>
        VRGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);
    }
    public class VRGameProviderService : IVRGameProviderService
    {
        private readonly IJsonSerializationProvider _json;
        private readonly ISettingOperations _settingOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;

        public VRGameProviderService(
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

        async ValueTask<GameProviderSchedule> IVRGameProviderService.GameProviderScheduleInitAsync(Guid gameProviderId, string brandCode, string merchantid)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var intervalMinuteMax = 24 * 60m;
            var intervalMinute = 20m;
            var config = new VRGameProviderScheduleConfig()
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
                GameProviderCode = GameProviderConst.VR,
                BrandCode = brandCode,
                AgentId = merchantid,
                CurrencyCode = string.Empty,
                Config = _json.SerializeToString(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
            };
            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            return gameProviderSchedule;
        }

        async ValueTask<GameProviderSchedule> IVRGameProviderService.GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, VRGPGameRecordResponse responses, DateTimeOffset sysTime)
        {
            var config = _json.DeserializeFromString<VRGameProviderScheduleConfig>(gameProviderSchedule.Config);
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
                return await GetGameProviderScheduleAsync(gameProviderSchedule.GameProviderId, gameProviderSchedule.BrandCode, gameProviderSchedule.AgentId).ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }

        async ValueTask<Guid> IVRGameProviderService.GetGameProviderIdAsync()
            => await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderConst.VR).ConfigureAwait(false);

        public async ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode, string merchantid)
            => await _gameProviderScheduleRepository.GetAsync(gameProviderId, brandCode, merchantid, string.Empty).ConfigureAwait(false);

        VRGameProviderScheduleConfig IVRGameProviderService.GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
            => _json.DeserializeFromString<VRGameProviderScheduleConfig>(gameProviderSchedule.Config);

        async ValueTask<VRConfiguration> IVRGameProviderService.GetSettingConfigurationAsync()
            => await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts.GameProviderConfigurations.VR).ConfigureAwait(false);

        string IVRGameProviderService.GetGameProviderCode()
            => GameProviderConst.VR;
    }
}
