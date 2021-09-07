using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
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
        ValueTask<BetData[]> BetAsync(TCGDebitRequest source);

        ValueTask<BatchBetData> SettleAsync(TCGCreditRequest source);

        ValueTask<BetData[]> ReserveMoneyAsync(TCGReserveMoneyRequest source);

        ValueTask<BatchBetData> CancelReserveAsync(TCGCancelReserveRequest source);

        ValueTask<BatchBetData> ConfirmReserveAsync(TCGConfirmReserveRequest source);
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

        async ValueTask<BetData[]> ITCGActionModelMapper.BetAsync(TCGDebitRequest source)
        {

            var playerMappers = new Dictionary<string, CachedPlayer>();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var gameProviderCode = _service.GetGameProviderCode();
            var cachedGame = await _service.GetCachedGameAsync(gameProviderId, source.ProductType);
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            var betDatas = new List<BetData>();

            var playerCodes = (from transactions in source.Transactions
                               group transactions
                               by transactions.UserName
                              into userNames
                               select userNames.Key
                              ).ToArray();

            foreach (var playerCode in playerCodes)
            {
                var batchTransactionData = new List<TransactionData>();
                var player = await _service.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(playerCode).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var filtersOrders = source.Transactions.Where(x => x.UserName == playerCode).ToList();

                var loginRecord = await _service.GetPlayerLatestEnabledLoginRecordAsync(playerCode).ConfigureAwait(false);
                filtersOrders.ForEach(order =>
                {
                    var additionalInformation = _jsonSerializationProvider.DeserializeFromString<AdditionalInformationLotto>(_jsonSerializationProvider.SerializeToString(order.AdditionalInformation));
                    batchTransactionData.Add(new TransactionData()
                    {
                        Amount = order.Amount,
                        ExternalTransactionId = order.RefNo.ToString(),
                        ExternalRoundId = additionalInformation.OrderNo,
                        ExternalBetId = order.RefNo.ToString(),
                        ExternalGameActionGroupId = order.RefNo.ToString(),
                        ExternalGameId = cachedGame.ExternalId,
                        GameActionType = GameActionType.PlaceBet,
                        TimeStamp = DateTimeOffset.UtcNow,
                        BetType = string.Empty
                    });
                });

                betDatas.Add(new BetData
                {
                    PlayerCode = playerCode,
                    PlatformType = loginRecord.PlatformType,
                    GameProviderId = gameProviderId,
                    GameProviderConfigurationId = gameProviderConfigurationId,
                    GameProviderCode = Consts.GameProvider.TCG,
                    HasCurrencyFromGameProvider = false,
                    AutoCloseRound = autoCloseRound,
                    AllowCancelCancelTransaction = false,
                    AllowPlaceBetWithDifferentPlatformToken = false,
                    Transactions = batchTransactionData.ToArray(),
                });
            }
            return betDatas.ToArray();
        }

        async ValueTask<BatchBetData> ITCGActionModelMapper.SettleAsync(TCGCreditRequest source)
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
                        ExternalBetId = bet.ExternalBetId.ToString(),
                        ExternalRoundId = additionalInformation.OrderNo,
                        ExternalTransactionId = order.RefNo.ToString(),
                        ExternalTransactionReferenceId = bet.ExternalReferencedTransactionId.ToString(),
                        GameActionType = (order.Amount > 0) ? GameActionType.WinBet : GameActionType.LoseBet,
                        BetType = string.Empty,
                        Timestamp = DateTime.UtcNow,
                        RoundDetail = _jsonSerializationProvider.SerializeToString(order)
                    });
                }
            }

            return GetBetData(gameProviderId, gameProviderCode, autoCloseRound, batchTransactionData.ToArray());
        }

        async ValueTask<BetData[]> ITCGActionModelMapper.ReserveMoneyAsync(TCGReserveMoneyRequest source)
        {
            var playerMappers = new Dictionary<string, CachedPlayer>();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var gameProviderCode = _service.GetGameProviderCode();
            var cachedGame = await _service.GetCachedGameAsync(gameProviderId, source.ProductType);
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            var betDatas = new List<BetData>();

            var playerCodes = (from transactions in source.Transactions
                               group transactions
                               by transactions.UserName
                              into userNames
                               select userNames.Key
                              ).ToArray();

            foreach (var playerCode in playerCodes)
            {
                var batchTransactionData = new List<TransactionData>();
                var player = await _service.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(playerCode).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var filtersOrders = source.Transactions.Where(x => x.UserName == playerCode).ToList();

                var loginRecord = await _service.GetPlayerLatestEnabledLoginRecordAsync(playerCode).ConfigureAwait(false);
                filtersOrders.ForEach(order =>
                {
                    var additionalInformation = _jsonSerializationProvider.DeserializeFromString<AdditionalInformationLotto>(_jsonSerializationProvider.SerializeToString(order.AdditionalInformation));
                    batchTransactionData.Add(new TransactionData()
                    {
                        Amount = order.Amount,
                        ExternalTransactionId = order.RefNo.ToString(),
                        ExternalRoundId = additionalInformation.OrderNo,
                        ExternalBetId = order.RefNo.ToString(),
                        ExternalGameActionGroupId = order.RefNo.ToString(),
                        ExternalGameId = cachedGame.ExternalId,
                        GameActionType = GameActionType.PlaceBet,
                        TimeStamp = DateTimeOffset.UtcNow,
                        BetType = string.Empty
                    });
                });

                betDatas.Add(new BetData
                {
                    PlayerCode = playerCode,
                    PlatformType = loginRecord.PlatformType,
                    GameProviderId = gameProviderId,
                    GameProviderConfigurationId = gameProviderConfigurationId,
                    GameProviderCode = Consts.GameProvider.TCG,
                    HasCurrencyFromGameProvider = false,
                    AutoCloseRound = autoCloseRound,
                    AllowCancelCancelTransaction = false,
                    AllowPlaceBetWithDifferentPlatformToken = false,
                    Transactions = batchTransactionData.ToArray(),
                });
            }
            return betDatas.ToArray();
        }

        async ValueTask<BatchBetData> ITCGActionModelMapper.CancelReserveAsync(TCGCancelReserveRequest source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var gameProviderCode = _service.GetGameProviderCode();
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            foreach (var order in source.ReservationDetails)
            {
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(order.UserName).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var additionalInformation = _jsonSerializationProvider.DeserializeFromString<AdditionalInformationLotto>(_jsonSerializationProvider.SerializeToString(order.AdditionalInformation));
                if (await _service.ReferencedGameActionExistsAsync(gameProviderId, order.UserName, additionalInformation.OrderNo, order.RefNo).ConfigureAwait(false))
                {
                    var actionExternalIds = await _service.GetGameActionExternalIdAsync(gameProviderId, order.UserName, order.RefNo).ConfigureAwait(false);
                    var externalTransactionId = $"{ order.RefNo}_cancel";
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = order.UserName,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        Amount = order.Amount,
                        Commission = 0,
                        ExternalGameId = actionExternalIds.ExternalGameId,
                        ExternalBetId = actionExternalIds.ExternalBetId.ToString(),
                        ExternalRoundId = actionExternalIds.ExternalRoundId,
                        ExternalTransactionId = externalTransactionId,
                        ExternalTransactionReferenceId = actionExternalIds.ExternalReferencedTransactionId.ToString(),
                        GameActionType = GameActionType.CancelTransaction,
                        BetType = string.Empty,
                        Timestamp = DateTime.UtcNow,
                        RoundDetail = _jsonSerializationProvider.SerializeToString(order)
                    });
                }

            }
            return GetBetData(gameProviderId, gameProviderCode, autoCloseRound, batchTransactionData.ToArray());
        }

        async ValueTask<BatchBetData> ITCGActionModelMapper.ConfirmReserveAsync(TCGConfirmReserveRequest source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var gameProviderCode = _service.GetGameProviderCode();
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            foreach (var order in source.ReservationDetails)
            {
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(order.UserName).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var additionalInformation = _jsonSerializationProvider.DeserializeFromString<AdditionalInformationLotto>(_jsonSerializationProvider.SerializeToString(order.AdditionalInformation));
                if (await _service.ReferencedGameActionExistsAsync(gameProviderId, order.UserName, additionalInformation.OrderNo, order.RefNo).ConfigureAwait(false))
                {
                    var actionExternalIds = await _service.GetGameActionExternalIdAsync(gameProviderId, order.UserName, order.RefNo).ConfigureAwait(false);
                    var externalTransactionId = $"{ order.RefNo}_cancel";
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = order.UserName,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        Amount = order.Amount,
                        Commission = 0,
                        ExternalGameId = actionExternalIds.ExternalGameId,
                        ExternalBetId = actionExternalIds.ExternalBetId.ToString(),
                        ExternalRoundId = actionExternalIds.ExternalRoundId,
                        ExternalTransactionId = externalTransactionId,
                        ExternalTransactionReferenceId = actionExternalIds.ExternalReferencedTransactionId.ToString(),
                        GameActionType = GameActionType.CancelTransaction,
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
