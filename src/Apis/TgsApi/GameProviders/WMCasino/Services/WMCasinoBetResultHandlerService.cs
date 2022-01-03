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
        private readonly IWMDataStore _store;
        private readonly ITgsLog _tgsLog;

        public WMCasinoBetResultHandlerService(
            ILogger<WMCasinoBetResultHandlerService> logger,
            IServiceProvider serviceProvider,
            IWMDataStore store,
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
            while (!stoppingToken.IsCancellationRequested)
            {
                var batchId = await _store.GetBatchIdAsync().ConfigureAwait(false);
                if (string.IsNullOrEmpty(batchId))
                {
                    await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
                    continue;
                }

                var betDetails =
                    (await _store
                        .GetBetResultsByBatchIdAsync(batchId)
                        .ConfigureAwait(false))
                    .ToArray();

                if (betDetails.Length == 0)
                {
                    continue;
                }

                using var scope = _serviceProvider.CreateScope();

                var service = scope.ServiceProvider.GetRequiredService<IWMCasinoBetActionServices>();

                try
                {
                    foreach (var betDetail in betDetails)
                    {
                        await service
                            .ExecuteBetResultsByRoundAsync(
                                betDetail.GameProviderId,
                                betDetail.GameProviderConfigurationId,
                                betDetail.PlayerCode,
                                new[] { betDetail.GameBetResult })
                            .ConfigureAwait(false);
                    }

                    await _store.RemoveBetResultsByBatchIdAsync(batchId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var value = JsonSerializer.Serialize(betDetails, _jsonSerializerOptions);

                    _tgsLog.LogError($"Fail to execute WMCasino bet result. BatchId: {batchId}, BetDetails: {value}", ex);
                    _logger.LogError(new EventId(1000), ex, $"Fail to execute WMCasino bet result: BatchId: {batchId}, BetDetails: {value}");
                }
            }
        }
    }
}
