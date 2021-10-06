using System;
using System.Collections.Generic;
using System.Linq;
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
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.Models.KY.Incoming;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.KY
{
    [Queue(Consts.Queue.GameProvider)]
    public sealed class KYGameRecordJobWorker : BaseWork<TgsResponse>
    {
        private readonly ITgsLog _log;
        private readonly IKYApiClient _client;
        private readonly IKYGameProviderService _service;
        private readonly IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository;

        public KYGameRecordJobWorker(
            ITgsLog log,
            IKYApiClient client,
            IKYGameProviderService service,
            IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository
            )
        {
            _log = log.SetLogger(nameof(KYGameRecordJobWorker));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            this._gameProviderScheduleLockRepository = _gameProviderScheduleLockRepository ?? throw new ArgumentNullException(nameof(_gameProviderScheduleLockRepository));
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
                    foreach (var agent in brand.AgentMappers.Where(x => x.Status))
                    {
                        Task t = Task.Run(async () =>
                        {
                            var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId, brand.BrandCode, agent.AgentId, agent.CurrencyCode).ConfigureAwait(false);
                            if (gameProviderSchedule == null)
                            {
                                gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId, brand.BrandCode, agent.AgentId, agent.CurrencyCode).ConfigureAwait(false);

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
        /// <param name="gameProvider">The game provider.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        private async Task GameRecordAsync(KYConfiguration configuration, GameProviderSchedule gameProviderSchedule)
        {
            var sysTime = DateTimeOffset.UtcNow;

            while (gameProviderSchedule != null)
            {
                var config = _service.GetGameProviderScheduleConfig(gameProviderSchedule);
                if (config.EndTime >= sysTime)
                {
                    break;
                }
                var responses = await _client.GameRecordAsync(configuration.ServiceEndpointUrl, new KYGPGameRecordRequest()
                {
                    BrandCode = gameProviderSchedule.BrandCode,
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
