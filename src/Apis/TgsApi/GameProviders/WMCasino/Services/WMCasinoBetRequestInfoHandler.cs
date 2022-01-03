using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using Tgs.MessageQueue;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Models;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public class WMCasinoBetRequestInfoHandler : IMessageHandler
    {
        private readonly IBrandOperations _brandOperations;
        private readonly ILogger<WMCasinoBetRequestInfoHandler> _logger;
        private readonly IPlayerOperations _playerOperations;
        private readonly IWMCasinoPlayerServices _playerServices;
        private readonly IWMDataStore _store;
        private readonly ITgsLog _tgsLog;

        public WMCasinoBetRequestInfoHandler(
            ILogger<WMCasinoBetRequestInfoHandler> logger,
            IWMDataStore store,
            IBrandOperations brandOperations,
            IPlayerOperations playerOperations,
            IWMCasinoPlayerServices playerServices,
            ITgsLog tgsLog)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _playerServices = playerServices ?? throw new ArgumentNullException(nameof(playerServices));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            string agentId = default;
            string roundNumber = default;

            try
            {
                var key = JsonSerializer.Deserialize<BetDetailRequestInfoKey>(data)!;
                agentId = key.AgentId;
                roundNumber = key.RoundNumber;

                var infos =
                    (await _store
                        .GetBetDetailRequestInfosAsync(agentId, roundNumber)
                        .ConfigureAwait(false))
                    .ToArray();

                if (infos.Length > 0)
                {
                    var gameProviderId = infos[0].GameProviderId;
                    var agent = infos[0].Agent;

                    var gameResults = infos.SelectMany(r => r.Request.Results).ToArray();
                    var results =
                            (from r in gameResults
                             group r by new { r.User } into g
                             select g)
                            .ToDictionary(
                                g => g.Key,
                                g => g.ToArray());

                    var configurationIds = new ConcurrentDictionary<string, Guid>();
                    var batchIds = new List<string>();

                    foreach (var r in results)
                    {
                        var playerCode = _playerServices.GetPlayerCode(r.Key.User, agent!.Prefix, agent!.Suffix);

                        if (!configurationIds.TryGetValue(playerCode, out var gameProviderConfigurationId))
                        {
                            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
                            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId)
                                .ConfigureAwait(false);

                            gameProviderConfigurationId = bgpc.GameProviderConfigurationId;
                            configurationIds[playerCode] = gameProviderConfigurationId;
                        }

                        var betDetails = r.Value
                            .Select(betResult =>
                                new BetDetail(
                                    gameProviderId,
                                    gameProviderConfigurationId,
                                    playerCode,
                                    betResult))
                            .ToArray();

                        var batchId = $"{playerCode}:{roundNumber}";
                        batchIds.Add(batchId);

                        await _store
                            .AddBetResultsToQueueAsync(batchId, betDetails)
                            .ConfigureAwait(false);
                    }

                    await _store
                        .ApplyBetResultBatchesAsync(batchIds)
                        .ConfigureAwait(false);
                }

                await _store.RemoveBetDetailRequestInfosAsync(agentId, roundNumber).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    new EventId(1000),
                    ex,
                    $"Fail to create bet detail player batches. AgentId: {agentId}, Round number: {roundNumber}");

                _tgsLog.LogError($"Fail to create bet detail player batches. AgentId: {agentId}, Round number: {roundNumber}", ex);
            }
        }
    }
}
