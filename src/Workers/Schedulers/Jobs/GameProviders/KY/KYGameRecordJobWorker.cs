using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Messages;
using TGS.GameProviders.Repositories.Models;
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

        public KYGameRecordJobWorker(
            ITgsLog log,
            IKYApiClient client,
            IKYGameProviderService service
            )
        {
            _log = log.SetLogger(nameof(KYGameRecordJobWorker));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
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
                            await GameRecordAsync(configuration, gameProviderSchedule);
                        });
                        tasks.Add(t);
                    }
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
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
