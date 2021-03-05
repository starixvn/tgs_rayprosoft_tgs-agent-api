using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Messages;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.SBOBET;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using GameProviderConst = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Workers.Schedulers.Jobs.GameProviders.SBOBET
{
    [Queue(Consts.Queue.GameProvider)]
    public sealed class CancelFailedTransactionsJobWorker : BaseWork<TgsResponse>
    {
        private const string TimeFormat = "yyyy/MM/ddTHH:mm:ss";
        private static readonly GameType[] GameTypes = {GameType.Sportsbook, GameType.VirtualSports};
        private static readonly TimeSpan Offset = TimeSpan.FromHours(-4);

        private readonly ISBOApiClient _client;
        private readonly IGameProviderCurrencyCodeMapper _currencyMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ITgsLog _log;
        private readonly IRoundQueries _roundQueries;
        private readonly ISettingOperations _settingOperations;

        public CancelFailedTransactionsJobWorker(
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations,
            IRoundQueries roundQueries,
            IGameProviderCurrencyCodeMapper currencyMapper,
            ISBOApiClient client,
            ITgsLog log)
        {
            _gameProviderOperations = gameProviderOperations;
            _settingOperations = settingOperations;
            _roundQueries = roundQueries;
            _currencyMapper = currencyMapper;
            _client = client;
            _log = log.SetLogger(nameof(CancelFailedTransactionsJobWorker));
        }

        public override async Task<TgsResponse> ExecuteAsync(object context)
        {
            try
            {
                var configuration =
                    await _settingOperations.GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts
                        .GameProviderConfigurations.SBOBET);

                if (!configuration.EnableCancelFailedTransactionsJob)
                {
                    return new()
                    {
                        Error = (int)TgsApiError.None,
                        ErrorMessage = "Job has been switched off"
                    };
                }

                var gameProviderId =
                    await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProviderConst.SBOBET);

                var start = DateTimeOffset.Now.AddMinutes(-35);
                var end = DateTimeOffset.Now.AddMinutes(-5);

                var roundExternalIds =
                    await _roundQueries.GetRoundWithGameActionExternalIdsAsync(gameProviderId, start, end,
                        RoundStatus.Open);

                if (roundExternalIds.Length == 0)
                {
                    return new();
                }

                var histories = await Task.WhenAll(GameTypes.Select(t =>
                {
                    var portfolio = Utility.GetPortfolio(t);
                    return _client.GetBetHistoryAsync(configuration.ApiUrl,
                        new()
                        {
                            CompanyKey = configuration.CompanyKey,
                            ServerId = "scheduler-sbo",
                            Portfolio = portfolio,
                            StartDate = start.ToOffset(Offset).ToString(TimeFormat),
                            EndDate = end.AddMinutes(5).ToOffset(Offset).ToString(TimeFormat)
                        });
                }));

                var historyExternalRoundIds = histories.Where(c => c.Results != null)
                    .SelectMany(c => c.Results).Select(r => r.ReferenceNo).ToArray();

                var failedRoundExternalIds =
                    roundExternalIds.Where(r => historyExternalRoundIds.IgnoreCaseNotContains(r.ExternalRoundId))
                        .Select(r => r).ToArray();

                var currencyMap = await _currencyMapper.GetMapByInternalCodeAsync(gameProviderId);

                var request = new BatchRequest
                {
                    ExternalBatchId = $"scheduler_sbo_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                    Timestamp = DateTimeOffset.UtcNow,
                    Transactions = failedRoundExternalIds.GroupBy(t => new
                        {
                            t.PlayerCode,
                            t.PlatformType
                        })
                        .SelectMany(g => g.SelectMany(r => r.GameActionExternalIds.Select(ga =>
                        {
                            if (!currencyMap.TryGetValue(r.CurrencyCode, out var curr))
                            {
                                curr = r.CurrencyCode;
                            }

                            return new BatchRequestTransaction
                            {
                                PlayerCode = g.Key.PlayerCode,
                                PlatformType = (int)g.Key.PlatformType,
                                CurrencyCode = curr,
                                Amount = ga.Amount.Negate(),
                                ExternalGameId = r.ExternalGameId,
                                ExternalTransactionId = $"{ga.ExternalTransactionId}_cancel",
                                ExternalTransactionReferenceId = ga.ExternalTransactionId,
                                ExternalBetId = ga.ExternalBetId,
                                ExternalRoundId = ga.ExternalRoundId,
                                RoundClosed = true
                            };
                        }))).ToArray()
                };

                await _client.CancelTransactionsAsync(configuration.ServiceEndpointUrl, request);
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