using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Messages;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.ScheduleConfig.VR;
using TGS.GameProviders.Repositories.Models.ScheduleConfig.WGSports;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.WGSports;
using TGS.Shared.Game.Models.VR.Incoming;
using TGS.Shared.Game.Models.WGSports.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;


namespace TGS.Workers.Schedulers.Jobs.GameProviders.WGSports
{
    [Queue(Consts.Queue.GameProvider)]
    public sealed class WGSportsGameRecordJobWorker : BaseWork<TgsResponse>
    {
        private readonly string _gameProviderCode = GameProviderConst.WGSports;
        private readonly string _gameProviderConfigurations = SettingConsts.GameProviderConfigurations.WGSports;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IRoundQueries _roundQueries;
        private readonly IGameProviderCurrencyCodeMapper _currencyMapper;
        private readonly IWGSportsApiClient _client;
        private readonly ITgsLog _log;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;

        public WGSportsGameRecordJobWorker(
           IGameProviderOperations gameProviderOperations,
           ISettingOperations settingOperations,
           IRoundQueries roundQueries,
           IGameProviderCurrencyCodeMapper currencyMapper,
           IWGSportsApiClient client,
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
            _log = log.SetLogger(nameof(WGSportsGameRecordJobWorker));
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
                var configuration =
                   await _settingOperations
                   .GetValueFromJsonByKeyAsync<WGSportsConfiguration>(_gameProviderConfigurations)
                   .ConfigureAwait(false);
                var gameProvider = await _gameProviderOperations
                      .GetGameProviderIdByCodeAsync(_gameProviderCode)
                      .ConfigureAwait(false);

                var tasks = new List<Task>();

                Task t = Task.Run(async () =>
                {
                    var gameProviderSchedule = await _gameProviderScheduleRepository.GetAsync(gameProvider, string.Empty, string.Empty, string.Empty).ConfigureAwait(false);
                    var config = default(WGSportsGameProviderScheduleConfig);
                    if (gameProviderSchedule == null)
                    {
                        var result = await GameProviderScheduleInit(gameProvider).ConfigureAwait(false);
                        gameProviderSchedule = result.gameProviderSchedule;
                        config = result.gameProviderScheduleConfig;
                    }
                    else
                    {
                        config = JsonSerializer.Deserialize<WGSportsGameProviderScheduleConfig>(gameProviderSchedule.Config);
                    }
                    await GameRecordAsync(configuration, gameProviderSchedule);
                });
                tasks.Add(t);
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
        private async Task GameRecordAsync(WGSportsConfiguration configuration, GameProviderSchedule gameProviderSchedule)
        {
            var sysTime = DateTimeOffset.UtcNow;

            while (gameProviderSchedule != null)
            {
                var config = JsonSerializer.Deserialize<WGSportsGameProviderScheduleConfig>(gameProviderSchedule.Config);
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
                    gameProviderSchedule = await GameProviderScheduleUpdateAsync(gameProviderSchedule, config, responses, sysTime).ConfigureAwait(false);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Games the provider schedule initialize.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="AgentId">The agent identifier.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns></returns>
        private async Task<(GameProviderSchedule gameProviderSchedule, WGSportsGameProviderScheduleConfig gameProviderScheduleConfig)> GameProviderScheduleInit(Guid gameProviderId)
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
                GameProviderCode = _gameProviderCode,
                BrandCode = string.Empty,
                AgentId = string.Empty,
                CurrencyCode = string.Empty,
                Config = JsonSerializer.Serialize(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
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
        private async Task<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, WGSportsGameProviderScheduleConfig config, WGSportsGPGameRecordResponse responses, DateTimeOffset sysTime)
        {
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
            gameProviderSchedule.Config = JsonSerializer.Serialize(config);
            gameProviderSchedule.UpdatedOn = DateTimeOffset.UtcNow;

            var updateResult = await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);
            if (updateResult.ModifiedCount > 0)
            {
                return gameProviderSchedule;
            }
            else
            {
                return null;
            }
        }
    }
}
