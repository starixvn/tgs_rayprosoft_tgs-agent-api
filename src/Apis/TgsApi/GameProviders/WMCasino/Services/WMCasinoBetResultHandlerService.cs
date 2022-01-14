using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.Logging;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public class WMCasinoBetResultHandlerService : BackgroundService
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<WMCasinoBetResultHandlerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWMCasinoDataStore _store;
        private readonly ITgsLog _tgsLog;

        public WMCasinoBetResultHandlerService(
            ILogger<WMCasinoBetResultHandlerService> logger,
            IServiceProvider serviceProvider,
            IWMCasinoDataStore store,
            ITgsLog tgsLog)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _tgsLog = tgsLog?.SetLogger(nameof(WMCasinoBetResultHandlerService)) ?? throw new ArgumentNullException(nameof(tgsLog));

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                var cancellationToken = cts.Token;

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation(new EventId(1000),
                            "start to get batch id");

                        var batchId = await _store.GetBatchIdAsync().ConfigureAwait(false);
                        if (string.IsNullOrEmpty(batchId))
                        {
                            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                            continue;
                        }

                        if (cancellationToken.IsCancellationRequested)
                        {
                            await _store.ApplyBetResultBatchesAsync(new[] { batchId }).ConfigureAwait(false);

                            _logger.LogInformation(new EventId(1000),
                                $"task end due to cancellation token is requested after getting batch id: {batchId}");

                            return;
                        }

                        _logger.LogInformation(new EventId(1000),
                            $"start to get bet details of batch id: {batchId}");

                        var betDetails =
                            (await _store
                                .GetBetResultsByBatchIdAsync(batchId)
                                .ConfigureAwait(false))
                            .ToArray();

                        if (betDetails.Length == 0)
                        {
                            continue;
                        }

                        if (cancellationToken.IsCancellationRequested)
                        {
                            await _store.ApplyBetResultBatchesAsync(new[] { batchId }).ConfigureAwait(false);

                            _logger.LogInformation(new EventId(1000),
                                $"task end due to cancellation token is requested after getting bet details of batch id:{batchId}");

                            return;
                        }

                        using var scope = _serviceProvider.CreateScope();

                        var service = scope.ServiceProvider.GetRequiredService<IWMCasinoBetActionServices>();

                        try
                        {
                            _logger.LogInformation(new EventId(1000),
                                $"start to execute bet details of batch id: {batchId}");

                            for (var i = 0; i < betDetails.Length; i++)
                            {
                                if (cancellationToken.IsCancellationRequested && i != betDetails.Length - 1)
                                {
                                    await _store.ApplyBetResultBatchesAsync(new[] { batchId }).ConfigureAwait(false);

                                    _logger.LogInformation(new EventId(1000),
                                        $"task end due to cancellation token is requested on executing bet details of batch id:{batchId}, sequence: {i + 1}");

                                    return;
                                }

                                var betDetail = betDetails[i];

                                await service
                                    .ExecuteBetResultsByRoundAsync(
                                        betDetail.GameProviderId,
                                        betDetail.GameProviderConfigurationId,
                                        betDetail.PlayerCode,
                                        new[] { betDetail.GameBetResult })
                                    .ConfigureAwait(false);
                            }

                            _logger.LogInformation(new EventId(1000),
                                $"end to execute bet details of batch id: {batchId}");

                            await _store.RemoveBetResultsByBatchIdAsync(batchId).ConfigureAwait(false);
                        }
                        catch (Exception inner)
                        {
                            var value = JsonSerializer.Serialize(betDetails, _jsonSerializerOptions);

                            _tgsLog.LogError(
                                $"Fail to execute WMCasino bet result. BatchId: {batchId}, BetDetails: {value}",
                                inner);
                            _logger.LogError(new EventId(5000),
                                inner,
                                $"Fail to execute WMCasino bet result: BatchId: {batchId}, BetDetails: {value}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(new EventId(5000),
                            ex,
                            $"Fail to execute {nameof(WMCasinoBetResultHandlerService)} service");

                        _tgsLog.LogError(
                            $"Fail to execute {nameof(WMCasinoBetResultHandlerService)} service",
                            ex);
                    }
                }
            }, stoppingToken);
        }
    }
}
