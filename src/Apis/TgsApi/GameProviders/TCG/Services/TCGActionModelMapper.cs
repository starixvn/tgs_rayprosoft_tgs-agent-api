using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.TCG.Common;
using TGS.Shared.Game.Models.TCG.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    public interface ITCGActionModelMapper
    {
        ValueTask<BatchBetData> BetAsync(TCGDebitRequest source);

        ValueTask<BatchBetData> SettleAsync(TCGDebitRequest source);
    }
    public class TCGActionModelMapper : ITCGActionModelMapper
    {
        private readonly ITCGService _service;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public TCGActionModelMapper(
            ITCGService service,
            IGameProviderOperations gameProviderOperations,
            IJsonSerializationProvider jsonSerializationProvider
            )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }
        async ValueTask<BatchBetData> ITCGActionModelMapper.BetAsync(TCGDebitRequest source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            var playerMappers = new Dictionary<string, CachedPlayer>();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var gameProviderCode = _service.GetGameProviderCode();
            var cachedGame = await _service.GetCachedGameAsync(gameProviderId, source.ProductType);
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            foreach (var order in source.Transactions)
            {
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(order.UserName).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var additionalInformation = _jsonSerializationProvider.DeserializeFromString<AdditionalInformationLotto>(_jsonSerializationProvider.SerializeToString(order.AdditionalInformation));

                batchTransactionData.Add(new BatchTransactionData()
                {
                    PlayerCode = order.UserName,
                    GameProviderConfigurationId = gameProviderConfigurationId,
                    Amount = order.Amount,
                    Commission = 0,
                    ExternalGameId = cachedGame.ExternalId,
                    ExternalBetId = order.RefNo.ToString(),
                    ExternalRoundId = additionalInformation.OrderNo,
                    ExternalTransactionId = order.RefNo.ToString(),
                    ExternalTransactionReferenceId = order.RefNo.ToString(),
                    GameActionType = GameActionType.PlaceBet,
                    BetType = string.Empty,
                    Timestamp = DateTime.UtcNow,
                    RoundDetail = _jsonSerializationProvider.SerializeToString(order)
                });
            }

            return GetBetData(gameProviderId, gameProviderCode, autoCloseRound, batchTransactionData.ToArray());
        }

        async ValueTask<BatchBetData> ITCGActionModelMapper.SettleAsync(TCGDebitRequest source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var gameProviderCode = _service.GetGameProviderCode();
            var cachedGame = await _service.GetCachedGameAsync(gameProviderId, source.ProductType);
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            foreach (var order in source.Transactions)
            {
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(order.UserName).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var additionalInformation = _jsonSerializationProvider.DeserializeFromString<AdditionalInformationLotto>(_jsonSerializationProvider.SerializeToString(order.AdditionalInformation));
                var bet = await _service.GetGameActionExternalIdByPlaceBetAsync(gameProviderId, order.UserName, additionalInformation.OrderNo, cachedGame.Id).ConfigureAwait(false);
                if (bet != null)
                {
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = order.UserName,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        Amount = order.Amount,
                        Commission = 0,
                        ExternalGameId = cachedGame.ExternalId,
                        ExternalBetId = order.RefNo.ToString(),
                        ExternalRoundId = additionalInformation.OrderNo,
                        ExternalTransactionId = order.RefNo.ToString(),
                        ExternalTransactionReferenceId = order.RefNo.ToString(),
                        GameActionType = GameActionType.PlaceBet,
                        BetType = string.Empty,
                        Timestamp = DateTime.UtcNow,
                        RoundDetail = _jsonSerializationProvider.SerializeToString(order)
                    });
                }
            }

            return GetBetData(gameProviderId, gameProviderCode, autoCloseRound, batchTransactionData.ToArray());
        }


        private BatchBetData GetBetData(Guid gameProviderId, string gameProviderCode, bool autoCloseRound, BatchTransactionData[] transactions)
          => new()
          {
              GameProviderId = gameProviderId,
              GameProviderCode = gameProviderCode,
              AutoCloseRound = autoCloseRound,
              ExternalBatchId = string.Empty,
              AllowCancelCancelTransaction = false,
              HasCurrencyFromGameProvider = false,
              Transactions = transactions,
          };

    }
}
