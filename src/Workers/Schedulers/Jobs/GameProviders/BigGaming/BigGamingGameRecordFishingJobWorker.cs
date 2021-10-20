using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hangfire;
using Tgs.Logging;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Messages;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.GameProviderScheduleLock;
using TGS.GameProviders.Repositories.Models.ScheduleConfig.BigGaming;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.BigGaming;
using TGS.Shared.Game.Models.BigGaming.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.BigGaming
{
    [Queue(Consts.Queue.GameProvider)]
    public sealed class BigGamingGameRecordFishingJobWorker : BaseWork<TgsResponse>
    {
        private readonly ITgsLog _log;
        private readonly IBigGamingApiClient _client;
        private readonly IBigGamingGameProviderFishingService _service;
        private readonly IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository;

        public BigGamingGameRecordFishingJobWorker(
          ITgsLog log,
          IBigGamingApiClient client,
          IBigGamingGameProviderFishingService service,
          IGameProviderScheduleLockRepository gameProviderScheduleLockRepository
           )
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
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

                var brandMerchants = configuration.BrandMerchants.Where(x => x.Status);

                foreach (var brand in brandMerchants)
                {

                    Task t = Task.Run(async () =>
                    {
                        var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId, brand.CurrencyCode).ConfigureAwait(false);
                        if (gameProviderSchedule == null)
                        {
                            gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId, brand.CurrencyCode).ConfigureAwait(false);

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
                            await GameRecordAsync(configuration, gameProviderSchedule);
                            await _gameProviderScheduleLockRepository.RemoveAsync(_service.GetGameProviderCode(), gameProviderSchedule.Id.ToString()).ConfigureAwait(false);
                        }
                    });
                    tasks.Add(t);

                }

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
        private async Task GameRecordAsync(BigGamingConfiguration configuration, GameProviderSchedule gameProviderSchedule)
        {
            var sysTime = DateTimeOffset.UtcNow;

            while (gameProviderSchedule != null)
            {
                var config = _service.GetGameProviderScheduleConfig(gameProviderSchedule);
                if (config.EndTime >= sysTime)
                {
                    break;
                }
                var responses = await _client.GameRecordAsync(configuration.ServiceEndpointUrl, new BigGamingGPGameRecordRequest()
                {
                    CurrencyCode = gameProviderSchedule.CurrencyCode,
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
