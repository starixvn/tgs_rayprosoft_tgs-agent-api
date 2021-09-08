using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.TCG.Common;
using TGS.Shared.Game.Models.TCG.Enums;
using TGS.Shared.Game.Models.TCG.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    public interface ITCGActionModelMapper
    {
        ValueTask<BetData[]> BetAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGDebitRequest source);

        ValueTask<BatchBetData> SettleAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGCreditRequest source);

        ValueTask<BetData[]> ReserveMoneyAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGReserveMoneyRequest source);

        ValueTask<BatchBetData> CancelReserveAsync(Guid gameProviderId, string gameProviderCode, TCGCancelReserveRequest source);

        ValueTask<BatchBetData> ConfirmReserveAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGConfirmReserveRequest source);

        ValueTask<QueryResult[]> CheckTransactionStatusAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGCheckTransactionStatusRequest request);
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

        async ValueTask<BetData[]> ITCGActionModelMapper.BetAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGDebitRequest source)
        {
            var playerMappers = new Dictionary<string, CachedPlayer>();
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

                if (!playerMappers.TryGetValue(playerCode, out CachedPlayer player))
                {
                    player = await _service.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);
                    playerMappers.Add(playerCode, player);
                }

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
                        BetType = string.Empty,
                        RoundDetail = _jsonSerializationProvider.SerializeToString(order)
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

        async ValueTask<BatchBetData> ITCGActionModelMapper.SettleAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGCreditRequest source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            foreach (var order in source.Transactions)
            {
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(order.UserName).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var additionalInformation = _jsonSerializationProvider.DeserializeFromString<AdditionalInformationLotto>(_jsonSerializationProvider.SerializeToString(order.AdditionalInformation));
                var betExternalId = await _service.GetGameActionExternalIdAsync(gameProviderId, order.UserName, order.RefNo).ConfigureAwait(false);
                if (betExternalId != null
                    && additionalInformation.OrderNo == betExternalId.ExternalRoundId
                    && betExternalId.GameActionType == GameActionType.PlaceBet
                    )
                {
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = order.UserName,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        Amount = order.Amount,
                        Commission = 0,
                        ExternalGameId = betExternalId.ExternalGameId,
                        ExternalBetId = betExternalId.ExternalBetId.ToString(),
                        ExternalRoundId = betExternalId.ExternalRoundId,
                        ExternalTransactionId = order.RefNo.ToString(),
                        ExternalTransactionReferenceId = betExternalId.ExternalTransactionId.ToString(),
                        GameActionType = (order.Amount > 0) ? GameActionType.WinBet : GameActionType.LoseBet,
                        BetType = string.Empty,
                        Timestamp = DateTime.UtcNow,
                        RoundDetail = _jsonSerializationProvider.SerializeToString(order)
                    });
                }
            }
            return GetBetData(gameProviderId, gameProviderCode, autoCloseRound, batchTransactionData.ToArray());
        }

        async ValueTask<BetData[]> ITCGActionModelMapper.ReserveMoneyAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGReserveMoneyRequest source)
        {
            var playerMappers = new Dictionary<string, CachedPlayer>();

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

                if (!playerMappers.TryGetValue(playerCode, out CachedPlayer player))
                {
                    player = await _service.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);
                    playerMappers.Add(playerCode, player);
                }

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
                        BetType = string.Empty,
                        RoundDetail = _jsonSerializationProvider.SerializeToString(order)
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

        async ValueTask<BatchBetData> ITCGActionModelMapper.CancelReserveAsync(Guid gameProviderId, string gameProviderCode, TCGCancelReserveRequest source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            foreach (var order in source.ReservationDetails)
            {
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(order.UserName).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var additionalInformation = _jsonSerializationProvider.DeserializeFromString<AdditionalInformationLotto>(_jsonSerializationProvider.SerializeToString(order.AdditionalInformation));
                var betExternalId = await _service.GetGameActionExternalIdAsync(gameProviderId, order.UserName, order.RefNo).ConfigureAwait(false);
                if (betExternalId != null
                    && additionalInformation.OrderNo == betExternalId.ExternalRoundId
                    && betExternalId.GameActionType == GameActionType.PlaceBet)
                {
                    var externalTransactionId = $"{ order.RefNo}_cancel";
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = order.UserName,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        Amount = order.Amount,
                        Commission = 0,
                        ExternalGameId = betExternalId.ExternalGameId,
                        ExternalBetId = betExternalId.ExternalBetId.ToString(),
                        ExternalRoundId = betExternalId.ExternalRoundId,
                        ExternalTransactionId = externalTransactionId,
                        ExternalTransactionReferenceId = betExternalId.ExternalTransactionId.ToString(),
                        GameActionType = GameActionType.CancelTransaction,
                        BetType = string.Empty,
                        Timestamp = DateTime.UtcNow,
                        RoundDetail = _jsonSerializationProvider.SerializeToString(order)
                    });
                }

            }
            return GetBetData(gameProviderId, gameProviderCode, autoCloseRound, batchTransactionData.ToArray());
        }

        async ValueTask<BatchBetData> ITCGActionModelMapper.ConfirmReserveAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGConfirmReserveRequest source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            foreach (var order in source.ReservationDetails)
            {
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(order.UserName).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var additionalInformation = _jsonSerializationProvider.DeserializeFromString<AdditionalInformationLotto>(_jsonSerializationProvider.SerializeToString(order.AdditionalInformation));
                var betExternalId = await _service.GetGameActionExternalIdAsync(gameProviderId, order.UserName, order.RefNo).ConfigureAwait(false);
                if (betExternalId != null
                    && additionalInformation.OrderNo == betExternalId.ExternalRoundId
                    && betExternalId.GameActionType == GameActionType.PlaceBet)
                {
                    var roundStatus = await _service.GetRoundStatusAsync(gameProviderId, order.UserName, betExternalId.ExternalRoundId, cachedGame.Id).ConfigureAwait(false);
                    if (roundStatus.Value == RoundStatus.Open)
                    {
                        var externalTransactionId = $"{ order.RefNo}_close";
                        batchTransactionData.Add(new BatchTransactionData()
                        {
                            PlayerCode = order.UserName,
                            GameProviderConfigurationId = gameProviderConfigurationId,
                            Amount = order.Amount,
                            Commission = 0,
                            ExternalGameId = betExternalId.ExternalGameId,
                            ExternalBetId = betExternalId.ExternalBetId.ToString(),
                            ExternalRoundId = betExternalId.ExternalRoundId,
                            ExternalTransactionId = externalTransactionId,
                            ExternalTransactionReferenceId = betExternalId.ExternalTransactionId.ToString(),
                            GameActionType = GameActionType.LoseBet,
                            BetType = string.Empty,
                            Timestamp = DateTime.UtcNow,
                            RoundDetail = _jsonSerializationProvider.SerializeToString(order)
                        });
                    }
                }
            }
            return GetBetData(gameProviderId, gameProviderCode, autoCloseRound, batchTransactionData.ToArray());
        }

        async ValueTask<QueryResult[]> ITCGActionModelMapper.CheckTransactionStatusAsync(Guid gameProviderId, string gameProviderCode, CachedGame cachedGame, TCGCheckTransactionStatusRequest request)
        {
            var queryResult = new List<QueryResult>();
            foreach (var transaction in request.TransactionDetails)
            {
                var externalId = await _service.GetGameActionExternalIdAsync(gameProviderId, transaction.UserName, transaction.RefNo).ConfigureAwait(false);
                if (externalId == null)
                {
                    queryResult.Add(new QueryResult
                    {
                        RefNo = transaction.RefNo,
                        TransStatus = TCGTransStatus.NOT_FOUND.ToString()
                    });
                    continue;
                }
                if (externalId.GameActionType != GameActionType.PlaceBet)
                {
                    queryResult.Add(new QueryResult
                    {
                        RefNo = transaction.RefNo,
                        TransStatus = TCGTransStatus.SUCCESS.ToString()
                    });
                    continue;
                }
                var gameActionData = await _service.GetGameActionsByExternalRoundIdAsync(gameProviderId, transaction.UserName, externalId.ExternalRoundId, cachedGame.Id).ConfigureAwait(false);
                if (gameActionData.GameActions.Length == 1)
                {
                    queryResult.Add(new QueryResult
                    {
                        RefNo = transaction.RefNo,
                        TransStatus = TCGTransStatus.RESERVED.ToString()
                    });
                    continue;
                }
                if (gameActionData.GameActions.Where(x => x.ExternalTransactionId.IgnoreCaseContains("_cancel")).FirstOrDefault() == null)
                {
                    queryResult.Add(new QueryResult
                    {
                        RefNo = transaction.RefNo,
                        TransStatus = TCGTransStatus.SUCCESS.ToString()
                    });

                }
                else
                {
                    queryResult.Add(new QueryResult
                    {
                        RefNo = transaction.RefNo,
                        TransStatus = TCGTransStatus.CANCELLED.ToString()
                    });
                }
            }
            return queryResult.ToArray();
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
