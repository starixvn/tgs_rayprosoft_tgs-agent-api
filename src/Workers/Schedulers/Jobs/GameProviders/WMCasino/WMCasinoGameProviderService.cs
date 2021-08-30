using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.ScheduleConfig.WMCasino;
using TGS.Shared.Game.Models.WMCasino.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.WMCasino
{
    public interface IWMCasinoGameProviderService
    {
        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode,
            string agentId, string currencyCode);

        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string brandCode,
            string agentId, string currencyCode);

        ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule,
            GameProviderGameRecordResponse response, DateTimeOffset sysTime);

        WMCasinoGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);
    }

    public class WMCasinoGameProviderService : IWMCasinoGameProviderService
    {
        private static readonly string _GameProviderCode = GameProviderConst.WMCasino;

        private readonly IJsonSerializationProvider _json;
        private readonly IGameProviderScheduleRepository _scheduleRepository;

        public WMCasinoGameProviderService(
            IJsonSerializationProvider json,
            IGameProviderScheduleRepository scheduleRepository)
        {
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        }

        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode,
            string agentId, string currencyCode)
            => _scheduleRepository.GetAsync(gameProviderId, brandCode, agentId, currencyCode);

        public async ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId,
            string brandCode, string agentId, string currencyCode)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var intervalMinuteMax = 24 * 60m;
            var intervalMinute = 20m;

            var config = new WMCasinoGameProviderScheduleConfig
            {
                IntervalMinuteMax = intervalMinuteMax,
                IntervalMinute = intervalMinute,
                LastGameRecordTime = new DateTimeOffset(Convert.ToDateTime("1970-01-01"), TimeSpan.FromHours(+8)),
                StartTime = sysTime.AddSeconds((double)intervalMinute * 60 * -1),
                EndTime = sysTime
            };

            var schedule = new GameProviderSchedule
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProviderId,
                GameProviderCode = _GameProviderCode,
                BrandCode = brandCode,
                AgentId = agentId,
                CurrencyCode = currencyCode,
                Config = _json.SerializeToString(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
            };

            await _scheduleRepository.MergeAsync(schedule).ConfigureAwait(false);

            return schedule;
        }

        public async ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(
            GameProviderSchedule gameProviderSchedule, GameProviderGameRecordResponse response, DateTimeOffset sysTime)
        {
            var config = GetGameProviderScheduleConfig(gameProviderSchedule);

            var intervalSeconds = (double)config.IntervalMinute * 60;
            var diffTime = sysTime - config.EndTime;

            if (config.EndTime > sysTime || diffTime.TotalSeconds > intervalSeconds)
            {
                config.StartTime = sysTime.AddSeconds(intervalSeconds * -1);
                config.EndTime = sysTime;
            }
            else
            {
                var endTime = config.EndTime.AddSeconds(intervalSeconds);
                config.StartTime = endTime;
                config.EndTime = endTime > sysTime ? sysTime : endTime;
            }

            config.LastGameRecordTime = response.LastGameRecordTime ?? config.LastGameRecordTime;
            gameProviderSchedule.Config = _json.SerializeToString(config);
            gameProviderSchedule.UpdatedOn = DateTimeOffset.UtcNow;

            var updateResult = await _scheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            if (updateResult.ModifiedCount == 0)
            {
                return null;
            }

            return await GetGameProviderScheduleAsync(
                gameProviderSchedule.GameProviderId,
                gameProviderSchedule.BrandCode,
                gameProviderSchedule.AgentId,
                gameProviderSchedule.CurrencyCode).ConfigureAwait(false);
        }

        public WMCasinoGameProviderScheduleConfig GetGameProviderScheduleConfig(
            GameProviderSchedule gameProviderSchedule)
            => _json.DeserializeFromString<WMCasinoGameProviderScheduleConfig>(gameProviderSchedule.Config);
    }
}