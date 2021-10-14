using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Tgs.Logging;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Messages;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.GameProviders.Repositories.Models.GameProviderScheduleLock;
using TGS.Shared.Game.ApiClients.TCG;
using TGS.Shared.Game.Models.TCG.Incoming;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.TCG
{
    [Queue(Consts.Queue.GameProvider)]
    public sealed class TCGGameRecordJobWorker : BaseWork<TgsResponse>
    {
        private readonly ITCGApiClient _client;
        private readonly IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository;
        private readonly ITgsLog _log;
        private readonly ITCGGameProviderService _service;

        public TCGGameRecordJobWorker(
            ITgsLog log,
            IGameProviderScheduleLockRepository gameProviderScheduleLockRepository,
            ITCGGameProviderService service,
            ITCGApiClient client
            )
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _gameProviderScheduleLockRepository = gameProviderScheduleLockRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleLockRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));
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
                var gameProviderCode = _service.GetGameProviderCode();
                var gameLotteryLobby = await _service.GetGameLotteryLobbyAsync(gameProviderId).ConfigureAwait(false);

                if (gameLotteryLobby == null)
                {
                    return new()
                    {
                        Error = (int)TgsApiError.SystemError,
                        ErrorMessage = "找不到TCG遊戲大廳"
                    };
                }
                var tasks = new List<Task>();
                foreach (var game in gameLotteryLobby)
                {
                    var merchants = configuration.Merchants;
                    if (game.AvailableCurrencies.Length > 0)
                    {
                        merchants = (from merchant in merchants
                                     join currency in game.AvailableCurrencies
                                     on merchant.CurrencyCode equals currency
                                     select merchant).ToArray();
                    }
                    if (merchants == null)
                    {
                        continue;
                    }

                    foreach (var merchant in merchants)
                    {
                        Task task = Task.Run(async () =>
                        {
                            var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId, game.Code, merchant.CurrencyCode, merchant.MerchantCode).ConfigureAwait(false);
                            if (gameProviderSchedule == null)
                            {
                                gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId, game.Code, merchant.CurrencyCode, merchant.MerchantCode).ConfigureAwait(false);
                            }
                            var gameProviderScheduleId = gameProviderSchedule.Id.ToString("N");
                            if (!await _gameProviderScheduleLockRepository.IsLockAsync(gameProviderCode, gameProviderScheduleId).ConfigureAwait(false))
                            {
                                await _gameProviderScheduleLockRepository.SetAsync(
                                    gameProviderCode,
                                    gameProviderScheduleId,
                                    new GameProviderScheduleLockData()
                                    {
                                        GameProviderCode = gameProviderCode,
                                        GameProviderId = gameProviderId,
                                        GameProviderScheduleId = gameProviderSchedule.Id,
                                        LockDate = DateTimeOffset.UtcNow
                                    }).ConfigureAwait(false);

                                await GameRecordAsync(configuration, gameProviderSchedule).ConfigureAwait(false);
                                await _gameProviderScheduleLockRepository.RemoveAsync(gameProviderCode, gameProviderScheduleId).ConfigureAwait(false);
                            }
                        });
                        tasks.Add(task);
                    }
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
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
        private async Task GameRecordAsync(TCGConfiguration configuration, GameProviderSchedule gameProviderSchedule)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var config = _service.GetGameProviderScheduleConfig(gameProviderSchedule);
            (string subGP, string merchantCode) = _service.GetSubGPAndMerchantCode(gameProviderSchedule.AgentId);
            var responses = await _client.GameRecordAsync(configuration.ServiceEndpointUrl, new TCGGPGameRecordRequest()
            {
                SubGP = subGP,
                MerchantCode = merchantCode,
                IsDesignation = false,
                LastGameRecorDirectory = config.LastGameRecorDirectory,
                LastGameRecorFile = config.LastGameRecorFile
            }).ConfigureAwait(false);

            if (responses.Success)
            {
                await _service.GameProviderScheduleUpdateAsync(gameProviderSchedule, responses, sysTime).ConfigureAwait(false);
            }
        }
    }
}
