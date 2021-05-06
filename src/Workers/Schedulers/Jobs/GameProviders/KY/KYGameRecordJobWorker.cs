using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GameProviders.Repositories;
using GameProviders.Repositories.Models;
using GameProviders.Repositories.Models.ScheduleConfig.KY;
using Hangfire;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Messages;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.Models.KY.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.KY
{
    [Queue(Consts.Queue.GameProvider)]
    public sealed class KYGameRecordJobWorker : BaseWork<TgsResponse>
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IRoundQueries _roundQueries;
        private readonly IGameProviderCurrencyCodeMapper _currencyMapper;
        private readonly IKYApiClient _client;
        private readonly ITgsLog _log;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;

        public KYGameRecordJobWorker(
           IGameProviderOperations gameProviderOperations,
           ISettingOperations settingOperations,
           IRoundQueries roundQueries,
           IGameProviderCurrencyCodeMapper currencyMapper,
           IKYApiClient client,
           ITgsLog log,
           IGameProviderScheduleRepository gameProviderScheduleRepository
           )
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _roundQueries = roundQueries ?? throw new ArgumentNullException(nameof(roundQueries));
            _currencyMapper = currencyMapper ?? throw new ArgumentNullException(nameof(currencyMapper));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameProviderScheduleRepository = gameProviderScheduleRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleRepository));
            _log = log.SetLogger(nameof(KYGameRecordJobWorker));
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
                var sysTime = DateTimeOffset.UtcNow;


                var configuration =
                   await _settingOperations
                   .GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts.GameProviderConfigurations.KY)
                   .ConfigureAwait(false);
                var gameProvider = await _gameProviderOperations
                      .GetGameProviderIdByCodeAsync(TGS.Core.Models.Consts.GameProvider.KY)
                      .ConfigureAwait(false);
                var currencys = await _gameProviderOperations
                      .CurrencySupportedAsync(gameProvider)
                      .ConfigureAwait(false);
                var tasks = new List<Task>();
                foreach (var currency in currencys)
                {
                    Task t = Task.Run(async () => { await GameRecordAsync(gameProvider, configuration, currency); });
                    tasks.Add(t);
                }
                Task.WaitAll(tasks.ToArray());
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
        private async Task GameRecordAsync(Guid gameProvider, KYConfiguration configuration, string currency)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var endTime = (DateTimeOffset?)null;
            while (!endTime.HasValue || endTime < sysTime)
            {
                var gameProviderSchedule = await _gameProviderScheduleRepository.GetAsync(gameProvider, currency).ConfigureAwait(false);
                var config = default(KYGameProviderScheduleConfig);
                if (gameProviderSchedule == null)
                {
                    var result = await GameProviderScheduleInit(gameProvider, currency).ConfigureAwait(false);
                    gameProviderSchedule = result.gameProviderSchedule;
                    config = result.gameProviderScheduleConfig;
                }
                else
                {
                    config = JsonSerializer.Deserialize<KYGameProviderScheduleConfig>(gameProviderSchedule.Config);
                }
                if (!config.Status)
                {
                    break;
                }
                var responses = await _client.GameRecordAsync(configuration.ServiceEndpointUrl, new KYGPGameRecordRequest()
                {
                    CurrencyCode = currency,
                    StartTime = config.StartTime.ToUnixTimeMilliseconds(),
                    EndTime = config.EndTime.ToUnixTimeMilliseconds()
                }).ConfigureAwait(false);
                if (responses.Success)
                {
                    endTime = await GameProviderScheduleUpdateAsync(gameProviderSchedule, config, responses).ConfigureAwait(false);
                }
                else
                {
                    break;
                }
                Thread.Sleep(15 * 10000);
            }
        }

        /// <summary>
        /// Games the provider schedule initialize.
        /// </summary>
        /// <param name="gameProvider">The game provider.</param>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        private async Task<(GameProviderSchedule gameProviderSchedule, KYGameProviderScheduleConfig gameProviderScheduleConfig)> GameProviderScheduleInit(Guid gameProvider, string currency)
        {
            var config = new KYGameProviderScheduleConfig()
            {
                Status = false,
                IntervalMinuteMax = 40m,
                IntervalMinute = 1.5m,
                LastGameRecorTime = new DateTimeOffset(Convert.ToDateTime("1970-01-01"), TimeSpan.FromHours(+8)),
                StartTime = new DateTimeOffset(Convert.ToDateTime("2021-05-05 08:00:00"), TimeSpan.FromHours(+0)),
                EndTime = new DateTimeOffset(Convert.ToDateTime("2021-05-05 08:01:30"), TimeSpan.FromHours(+0))
            };

            var gameProviderSchedule = new GameProviderSchedule()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProvider,
                GameProviderCode = TGS.Core.Models.Consts.GameProvider.KY,
                CurrencyCode = currency,
                Config = JsonSerializer.Serialize(config),
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedOn = DateTimeOffset.UtcNow
            };
            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            return (gameProviderSchedule, config);
        }

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="value">The value.</param>
        /// <param name="responses">The responses.</param>
        /// <returns></returns>
        private async Task<DateTimeOffset> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, KYGameProviderScheduleConfig config, KYGPGameRecordResponses responses)
        {
            config.LastGameRecorTime = (responses.LastGameRecorTime.HasValue) ? responses.LastGameRecorTime.Value : config.LastGameRecorTime;
            config.StartTime = config.EndTime;
            config.EndTime = config.StartTime.AddSeconds((double)config.IntervalMinute * 60);
            gameProviderSchedule.Config = JsonSerializer.Serialize(config);
            gameProviderSchedule.UpdatedOn = DateTimeOffset.UtcNow;

            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);
            return config.EndTime;
        }
    }


}
