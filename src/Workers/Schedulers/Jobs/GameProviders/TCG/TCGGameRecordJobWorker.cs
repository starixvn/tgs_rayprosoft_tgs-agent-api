using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models.GameProviderScheduleLock;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.TCG
{
    [Queue(Consts.Queue.GameProvider)]
    public sealed class TCGGameRecordJobWorker : BaseWork<TgsResponse>
    {
        private readonly ITgsLog _log;
        private readonly IGameProviderScheduleLockRepository _gameProviderScheduleLockRepository;
        private readonly ITCGGameProviderService _service;

        public TCGGameRecordJobWorker(
            ITgsLog log,
            IGameProviderScheduleLockRepository gameProviderScheduleLockRepository,
            ITCGGameProviderService service
            )
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _gameProviderScheduleLockRepository = gameProviderScheduleLockRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleLockRepository));
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
                var config = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
                var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
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
                    var merchants = config.Merchants;
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
                        Task t = Task.Run(async () =>
                        {
                            var gameProviderSchedule = await _service.GetGameProviderScheduleAsync(gameProviderId, game.Code, merchant.CurrencyCode, merchant.MerchantCode).ConfigureAwait(false);
                            if (gameProviderSchedule == null)
                            {
                                gameProviderSchedule = await _service.GameProviderScheduleInitAsync(gameProviderId, game.Code, merchant.CurrencyCode, merchant.MerchantCode).ConfigureAwait(false);
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
                                //await GameRecordAsync(configuration, gameProviderSchedule);
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

                _log.LogError("Execute job with error:", e);

                return new()
                {
                    Error = (int)TgsApiError.SystemError,
                    ErrorMessage = e.Message
                };
            }
            return new();
        }
    }
}
