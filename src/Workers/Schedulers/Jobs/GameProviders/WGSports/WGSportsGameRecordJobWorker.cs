using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using Tgs.Logging;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Messages;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.GameProviderScheduleLock;
using TGS.Shared.Game.ApiClients.WGSports;
using TGS.Shared.Game.Models.WGSports.Incoming;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.WGSports
{
    [Queue(Consts.Queue.GameProvider)]
    public sealed class WGSportsGameRecordJobWorker : BaseWork<TgsResponse>
    {
        private readonly ITgsLog _log;
        private readonly IWGSportsApiClient _client;
        private readonly IWGSportsGameProviderService _service;
        private readonly IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository;

        public WGSportsGameRecordJobWorker(
             ITgsLog log,
             IWGSportsApiClient client,
             IWGSportsGameProviderService service,
             IGameProviderScheduleLockRepository gameProviderScheduleLockRepository
           )
        {
            _log = log.SetLogger(nameof(WGSportsGameRecordJobWorker));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameProviderScheduleLockRepository = gameProviderScheduleLockRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleLockRepository));
        }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<TgsResponse> ExecuteAsync(object context)
        {
            try
            {
                var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

                var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

                var tasks = new List<Task>();

                Task t = Task.Run(async () =>
                {
                    var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId).ConfigureAwait(false);
                    if (gameProviderSchedule == null)
                    {
                        gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId).ConfigureAwait(false);
                    }
                    if (!await _gameProviderScheduleLockRepository.IsLockAsync(_service.GetGameProviderCode(), gameProviderSchedule.Id.ToString()).ConfigureAwait(false))
                    {
                        await _gameProviderScheduleLockRepository.SetAsync(
                            _service.GetGameProviderCode(),
                            gameProviderSchedule.Id.ToString(),
                            new GameProviderScheduleLockData()
                            {
                                GameProviderCode = _service.GetGameProviderCode(),
                                GameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false),
                                GameProviderScheduleId = gameProviderSchedule.Id,
                                LockDate = DateTimeOffset.UtcNow
                            }).ConfigureAwait(false);
                        await GameRecordAsync(configuration, gameProviderSchedule).ConfigureAwait(false);
                        await _gameProviderScheduleLockRepository.RemoveAsync(_service.GetGameProviderCode(), gameProviderSchedule.Id.ToString()).ConfigureAwait(false);
                    }
                });
                tasks.Add(t);
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                await _gameProviderScheduleLockRepository.RemoveGameProviderCodeAsync(_service.GetGameProviderCode()).ConfigureAwait(false);

                _log.LogError("Execute job with error:", e);

                return new()
                {
                    Error = (int)TgsApiError.SystemError,
                    ErrorMessage = e.Message
                };
            }
            return new();
        }

        /// <summary>
        /// Games the record asynchronous.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <returns></returns>
        private async Task GameRecordAsync(WGSportsConfiguration configuration, GameProviderSchedule gameProviderSchedule)
        {
            var sysTime = DateTimeOffset.UtcNow;

            while (gameProviderSchedule != null)
            {
                var config = _service.GetGameProviderScheduleConfig(gameProviderSchedule);

                if (config.EndTime >= sysTime)
                {
                    break;
                }

                var responses = await _client.GameRecordAsync(configuration.ServiceEndpointUrl, new WGSportsGPGameRecordRequest()
                {
                    StartTime = config.StartTime,
                    EndTime = config.EndTime
                }).ConfigureAwait(false);

                if (responses.Success)
                {
                    gameProviderSchedule = await _service.GameProviderScheduleUpdateAsync(gameProviderSchedule, responses, sysTime).ConfigureAwait(false);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
