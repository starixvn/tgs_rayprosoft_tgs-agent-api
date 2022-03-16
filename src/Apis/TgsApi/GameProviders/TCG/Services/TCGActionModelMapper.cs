using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.TCG.Common;
using TGS.Shared.Game.Models.TCG.Enums;
using TGS.Shared.Game.Models.TCG.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    public class TCGActionModelMapper : ITCGActionModelMapper
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ILogger<TCGActionModelMapper> _logger;
        private readonly ITCGService _service;

        public TCGActionModelMapper(
            IGameProviderOperations gameProviderOperations,
            ILogger<TCGActionModelMapper> logger,
            ITCGService service
            )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<BetData[]> BetAsync(Guid gameProviderId, CachedGame cachedGame, TCGDebitRequest source)
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
                    var additionalInformation = JsonSerializer.Deserialize<AdditionalInformationLotto>(JsonSerializer.Serialize(order.AdditionalInformation));
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
                        RoundDetail = JsonSerializer.Serialize(order)
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

        public async ValueTask<BatchBetData> CancelReserveAsync(Guid gameProviderId, TCGCancelReserveRequest source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            foreach (var order in source.ReservationDetails)
            {
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(order.UserName).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var additionalInformation = JsonSerializer.Deserialize<AdditionalInformationLotto>(JsonSerializer.Serialize(order.AdditionalInformation));
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
                        RoundDetail = JsonSerializer.Serialize(order)
                    });
                }
            }
            return GetBetData(gameProviderId, autoCloseRound, batchTransactionData.ToArray());
        }

        public async ValueTask<QueryResult> CheckTransactionStatusAsync(Guid gameProviderId, string player, string externalTransactionId)
        {
            var externalId = await _service.GetGameActionExternalIdAsync(gameProviderId, player, externalTransactionId).ConfigureAwait(false);
            if (externalId == null)
            {
                return new QueryResult
                {
                    RefNo = externalTransactionId,
                    TransStatus = TCGTransStatus.NOT_FOUND.ToString()
                };
            }
            return new QueryResult
            {
                RefNo = externalTransactionId,
                TransStatus = TCGTransStatus.SUCCESS.ToString()
            };
        }

        public async ValueTask<BetData> CloseAsync(TCGCloseRequest source)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var betExternalId = await _service.GetGameActionExternalIdAsync(gameProviderId, source.UserName, source.RefNo).ConfigureAwait(false);

            if (betExternalId == null
                || betExternalId.GameActionType != GameActionType.PlaceBet)
            {
                return null;
            }

            var cachedGame = await _service.GetCachedGameAsync(betExternalId.ExternalGameId).ConfigureAwait(false);

            var roundStatus = await _service.GetRoundStatusAsync(
                gameProviderId, source.UserName,
                betExternalId.ExternalRoundId,
                cachedGame.Id).ConfigureAwait(false);
            if (roundStatus.Value != RoundStatus.Open)
            {
                return null;
            }

            var brandId = await _service.GetBrandIdByPlayerCodeAsync(
                source.UserName).ConfigureAwait(false);

            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(
                gameProviderId).ConfigureAwait(false);

            var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(
               brandId,
               gameProviderId).ConfigureAwait(false);
            var loginRecord = await _service.GetPlayerLatestEnabledLoginRecordAsync(source.UserName).ConfigureAwait(false);
            var externalTransactionId = $"{ source.RefNo}_close";
            var transaction = new TransactionData()
            {
                Amount = 0,
                ExternalTransactionId = externalTransactionId,
                ExternalRoundId = betExternalId.ExternalRoundId,
                ExternalBetId = betExternalId.ExternalBetId.ToString(),
                ExternalGameActionGroupId = betExternalId.ExternalGameActionGroupId.ToString(),
                ExternalTransactionReferenceId = betExternalId.ExternalBetId.ToString(),
                ExternalGameId = betExternalId.ExternalGameId,
                GameActionType = GameActionType.LoseBet,
                TimeStamp = DateTimeOffset.UtcNow,
                BetType = string.Empty,
                RoundDetail = JsonSerializer.Serialize(source.AdditionalInformation)
            };
            return new BetData
            {
                PlayerCode = (loginRecord != null) ? loginRecord.PlayerCode : source.UserName,
                PlatformType = (loginRecord != null) ? loginRecord.PlatformType : PlatformType.Desktop,
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.TCG,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = autoCloseRound,
                AllowCancelCancelTransaction = false,
                AllowPlaceBetWithDifferentPlatformToken = false,
                Transactions = new TransactionData[] { transaction },
            };
        }

        public async ValueTask<BatchBetData> ConfirmReserveAsync(Guid gameProviderId, TCGConfirmReserveRequest source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            foreach (var order in source.ReservationDetails)
            {
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(order.UserName).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var additionalInformation = JsonSerializer.Deserialize<AdditionalInformationLotto>(JsonSerializer.Serialize(order.AdditionalInformation));
                var betExternalId = await _service.GetGameActionExternalIdAsync(gameProviderId, order.UserName, order.RefNo).ConfigureAwait(false);

                if (betExternalId != null
                    && additionalInformation.OrderNo == betExternalId.ExternalRoundId
                    && betExternalId.GameActionType == GameActionType.PlaceBet)
                {
                    var cachedGame = await _service.GetCachedGameAsync(betExternalId.ExternalGameId).ConfigureAwait(false);
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
                            RoundDetail = JsonSerializer.Serialize(order)
                        });
                    }
                }
            }
            return GetBetData(gameProviderId, autoCloseRound, batchTransactionData.ToArray());
        }

        public async ValueTask<BetData[]> ReserveMoneyAsync(Guid gameProviderId, CachedGame cachedGame, TCGReserveMoneyRequest source)
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
                    var additionalInformation = JsonSerializer.Deserialize<AdditionalInformationLotto>(JsonSerializer.Serialize(order.AdditionalInformation));
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
                        RoundDetail = JsonSerializer.Serialize(order)
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

        public async ValueTask<BatchBetData> SettleAsync(Guid gameProviderId, IEnumerable<CachedGame> cachedGames, TCGCreditRequest source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId).ConfigureAwait(false);
            foreach (var order in source.Transactions)
            {
                CachedPlayer player = default;
                try
                {
                    player = await _service.GetCachedPlayerAsync(order.UserName).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(JsonSerializer.Serialize(new
                    {
                        Data = JsonSerializer.Serialize(order),
                        Message = $"玩家不存在{ex.Message}",
                        Logon = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    }));
                    continue;
                }
                var brandId = await _service.GetBrandIdByPlayerCodeAsync(player.PlayerCode).ConfigureAwait(false);
                var gameProviderConfigurationId = await _service.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
                var additionalInformation = JsonSerializer.Deserialize<AdditionalInformationLotto>(JsonSerializer.Serialize(order.AdditionalInformation));

                var cachedGame = default(CachedGame);
                var roundRecord = default(RoundRecord);
                var gameActionRecord = default(GameActionRecord);
                foreach (var game in cachedGames)
                {
                    (roundRecord, gameActionRecord) = await _service.GetGameActionsByBetAsync(gameProviderId, player.PlayerCode, additionalInformation.OrderNo, game.Id).ConfigureAwait(false);
                    if (gameActionRecord != null
                        && additionalInformation.OrderNo == roundRecord.ExternalRoundId
                        )
                    {
                        cachedGame = game;
                        break;
                    }
                }

                if (gameActionRecord != null && roundRecord != null && cachedGame != null)
                {
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = player.PlayerCode,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        Amount = order.Amount,
                        Commission = 0,
                        ExternalGameId = cachedGame.ExternalId,
                        ExternalBetId = gameActionRecord.ExternalBetId.ToString(),
                        ExternalRoundId = roundRecord.ExternalRoundId,
                        ExternalTransactionId = order.RefNo.ToString(),
                        ExternalTransactionReferenceId = gameActionRecord.ExternalTransactionId.ToString(),
                        GameActionType = (order.Amount > 0) ? GameActionType.WinBet : GameActionType.LoseBet,
                        BetType = string.Empty,
                        Timestamp = DateTime.UtcNow,
                        RoundDetail = JsonSerializer.Serialize(order)
                    });
                }
            }
            return GetBetData(gameProviderId, autoCloseRound, batchTransactionData.ToArray());
        }

        private BatchBetData GetBetData(Guid gameProviderId, bool autoCloseRound, BatchTransactionData[] transactions)
         => new()
         {
             GameProviderId = gameProviderId,
             GameProviderCode = Consts.GameProvider.TCG,
             AutoCloseRound = autoCloseRound,
             ExternalBatchId = string.Empty,
             AllowCancelCancelTransaction = false,
             HasCurrencyFromGameProvider = false,
             Transactions = transactions,
         };
    }
}
