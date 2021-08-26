using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.WMCasino;
using TGS.Core.Models.Messages;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.GameProviderScheduleLock;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.WMCasino;
using TGS.Shared.Game.Models.WMCasino.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.WMCasino
{
    [Queue(Consts.Queue.GameProvider)]
    public class WMCasinoGameRecordJobWorker : BaseWork<TgsResponse>
    {
        private static readonly string _GameProviderCode = GameProviderConst.WMCasino;
        private readonly IWMCasinoApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ITgsLog _log;
        private readonly IGameProviderScheduleLockRepository _scheduleLockRepository;
        private readonly IWMCasinoGameProviderService _service;
        private readonly ISettingOperations _settingOperations;

        public WMCasinoGameRecordJobWorker(
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderScheduleLockRepository scheduleLockRepository,
            IWMCasinoGameProviderService service,
            IWMCasinoApiClient client,
            ITgsLog log)
        {
            _log = log?.SetLogger(nameof(WMCasinoGameRecordJobWorker)) ??
                throw new ArgumentNullException(nameof(log));

            _settingOperations = settingOperations ??
                throw new ArgumentNullException(nameof(settingOperations));

            _gameProviderOperations = gameProviderOperations ??
                throw new ArgumentNullException(nameof(gameProviderOperations));

            _scheduleLockRepository = scheduleLockRepository ??
                throw new ArgumentNullException(nameof(scheduleLockRepository));

            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public override async Task<TgsResponse> ExecuteAsync(object context)
        {
            try
            {
                var configuration = await _settingOperations
                    .GetValueFromJsonByKeyAsync<WMCasinoConfiguration>(
                        SettingConsts.GameProviderConfigurations.WMCasino).ConfigureAwait(false);

                var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(_GameProviderCode)
                    .ConfigureAwait(false);

                var tasks = configuration.BrandMerchants.SelectMany(b =>
                {
                    var brandCode = b.BrandCode;

                    return b.Agents.Select(a => Task.Run(async () =>
                        await ExecuteScheduleAsync(gameProviderId, brandCode, a, configuration)
                            .ConfigureAwait(false)));
                });

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _scheduleLockRepository.RemoveGameProviderCodeAsync(_GameProviderCode)
                    .ConfigureAwait(false);

                _log.LogError("Execute job with error:", ex);

                return new TgsResponse
                {
                    Error = (int)TgsApiError.SystemError,
                    ErrorMessage = ex.Message
                };
            }

            return new TgsResponse();
        }

        private async ValueTask ExecuteScheduleAsync(Guid gameProviderId, string brandCode, WMCasinoAgent agent,
            WMCasinoConfiguration configuration)
        {
            var schedule = await _service
                .GetGameProviderScheduleAsync(gameProviderId, brandCode, agent.AgentId, agent.CurrencyCode)
                .ConfigureAwait(false);

            if (schedule == null)
            {
                schedule = await _service
                    .GameProviderScheduleInitAsync(gameProviderId, brandCode, agent.AgentId, agent.CurrencyCode)
                    .ConfigureAwait(false);
            }

            var scheduleIdStr = schedule.Id.ToString();
            var isLock =
                await _scheduleLockRepository.IsLockAsync(_GameProviderCode, scheduleIdStr);

            if (isLock)
            {
                return;
            }

            await _scheduleLockRepository.SetAsync(
                _GameProviderCode,
                scheduleIdStr,
                new GameProviderScheduleLockData
                {
                    GameProviderCode = _GameProviderCode,
                    GameProviderId = gameProviderId,
                    GameProviderScheduleId = schedule.Id,
                    LockDate = DateTimeOffset.UtcNow
                }).ConfigureAwait(false);

            await GameRecordAsync(configuration, schedule);
            await _scheduleLockRepository.RemoveAsync(_GameProviderCode, scheduleIdStr)
                .ConfigureAwait(false);
        }

        private async ValueTask GameRecordAsync(WMCasinoConfiguration configuration, GameProviderSchedule schedule)
        {
            var sysTime = DateTimeOffset.UtcNow;

            while (schedule != null)
            {
                var config = _service.GetGameProviderScheduleConfig(schedule);

                if (config.EndTime >= sysTime)
                {
                    break;
                }

                var response =
                    await _client.GetGameRecordAsync(configuration.ServiceEndpointUrl,
                        new GameProviderGameRecordRequest
                        {
                            BrandCode = schedule.BrandCode,
                            CurrencyCode = schedule.CurrencyCode,
                            AgentId = schedule.AgentId,
                            StartTime = config.StartTime,
                            EndTime = config.EndTime
                        });

                if (!response.Success)
                {
                    break;
                }

                schedule = await _service.GameProviderScheduleUpdateAsync(schedule, response, sysTime)
                    .ConfigureAwait(false);
            }
        }
    }
}