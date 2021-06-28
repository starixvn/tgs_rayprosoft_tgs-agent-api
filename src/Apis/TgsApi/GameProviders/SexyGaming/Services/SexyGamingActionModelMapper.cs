using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions;
using TGS.Shared.Game.Models.SexyGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public interface ISexyGamingActionModelMapper
    {
        Task<BetData> BetAsync(GameProviderConfigurationData data, string playCode, SexyGamingBet[] source);

        Task<BetData> CancelBetAsync(GameProviderConfigurationData data, string playCode, SexyGamingCancelBet[] source);

        Task<BatchBetData> VoidSettleAsync(GameProviderConfigurationData data, string playCode, SexyGamingVoidSettle[] source);

        Task<BatchBetData> SettleAsync(GameProviderConfigurationData data, SexyGamingSettle[] source, OppositeBetSetting[] oppositeBetSettings);

        Task<BatchBetData> UnsettleAsync(GameProviderConfigurationData data, SexyGamingUnsettle[] source);

        Task<BatchBetData> VoidBetAsync(GameProviderConfigurationData data, SexyGamingVoidBett[] source);

    }

    public class SexyGamingActionModelMapper : ISexyGamingActionModelMapper
    {
        private const string GameProviderCode = Consts.GameProvider.SexyGaming;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ISexyGamingServices _services;
        private readonly ISexyGamingRedisCacheService _redisCacheService;

        public SexyGamingActionModelMapper(
            ISexyGamingServices services,
            IJsonSerializationProvider jsonSerializationProvider,
            ISexyGamingRedisCacheService redisCacheService
            )
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        }


        async Task<BetData> ISexyGamingActionModelMapper.BetAsync(GameProviderConfigurationData data, string playCode, SexyGamingBet[] source)
        {
            var platformTxIdJoin = await _redisCacheService.GetCancelBetPlatformTxIdJoinAsync(source).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(platformTxIdJoin))
            {
                return null;
            }

            var transactions = (from order in source
                                select new TransactionData
                                {
                                    Amount = Math.Abs(order.BetAmount),
                                    ExternalTransactionId = order.PlatformTxId.ToString(),
                                    ExternalRoundId = order.RoundId,
                                    ExternalBetId = order.PlatformTxId.ToString(),
                                    ExternalGameActionGroupId = order.PlatformTxId.ToString(),
                                    ExternalGameId = order.GameCode.ToString(),
                                    GameActionType = GameActionType.PlaceBet,
                                    ExtraData = order.BetTime.ToIso8601String(true),
                                    TimeStamp = DateTimeOffset.UtcNow,
                                    BetType = order.BetType
                                }).ToArray();
            return GetBetData(data, playCode, transactions);
        }

        async Task<BetData> ISexyGamingActionModelMapper.CancelBetAsync(GameProviderConfigurationData data, string playCode, SexyGamingCancelBet[] source)
        {
            var batchTransactionData = new List<TransactionData>();

            await _redisCacheService.AddCancelBetCacheAsync(source).ConfigureAwait(false);

            foreach (var order in source)
            {
                var bet = await _services.GetGameActionExternalIdAsync(data.GameProviderId, order.UserId, $"{ order.PlatformTxId}").ConfigureAwait(false);
                if (bet != null)
                {
                    batchTransactionData.Add(new TransactionData()
                    {
                        Amount = 0,
                        Commission = 0,
                        ExternalGameId = order.GameCode,
                        ExternalBetId = order.PlatformTxId,
                        ExternalRoundId = order.RoundId,
                        ExternalTransactionId = $"{order.PlatformTxId}_cancel_bet",
                        ExternalTransactionReferenceId = order.PlatformTxId,
                        GameActionType = GameActionType.CancelTransaction,
                        TimeStamp = DateTimeOffset.UtcNow
                    });
                }
            }

            return GetBetData(data, playCode, batchTransactionData.ToArray());
        }

        async Task<BatchBetData> ISexyGamingActionModelMapper.SettleAsync(GameProviderConfigurationData data, SexyGamingSettle[] source, OppositeBetSetting[] oppositeBetSettings)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            foreach (var order in source)
            {
                var brandId = await _services.GetBrandIdByPlayerCodeAsync(order.UserId).ConfigureAwait(false);
                var gameProviderConfigurationId = await _services.GetGameProviderConfigurationIdAsync(brandId, data.GameProviderId).ConfigureAwait(false);
                var settle = await _services.GetGameActionExternalIdAsync(data.GameProviderId, order.UserId, $"{ order.PlatformTxId}_settle").ConfigureAwait(false);
                var externalTransactionId = $"{ order.PlatformTxId}_settle";
                if (settle != null)
                {
                    var settleCancel = await _services.GetGameActionExternalIdAsync(data.GameProviderId, order.UserId, $"{ order.PlatformTxId}_settle_cancel").ConfigureAwait(false);
                    if (settleCancel != null)
                    {
                        externalTransactionId = $"{ order.PlatformTxId}_resettle";
                    }
                }
                batchTransactionData.Add(new BatchTransactionData()
                {
                    PlayerCode = order.UserId,
                    GameProviderConfigurationId = gameProviderConfigurationId,
                    Amount = Math.Abs(order.WinAmount),
                    Commission = 0,
                    ExternalGameId = order.GameCode,
                    ExternalBetId = order.PlatformTxId.ToString(),
                    ExternalRoundId = order.RoundId.ToString(),
                    ExternalTransactionId = externalTransactionId,
                    ExternalTransactionReferenceId = order.PlatformTxId,
                    GameActionType = ProcessingSettlement(order),
                    BetType = order.BetType,
                    Timestamp = DateTime.UtcNow,
                    RoundDetail = _jsonSerializationProvider.SerializeToString(order.GameInfo)
                });
            }

            return GetBetData(data, batchTransactionData.ToArray());
        }

        async Task<BatchBetData> ISexyGamingActionModelMapper.UnsettleAsync(GameProviderConfigurationData data, SexyGamingUnsettle[] source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            foreach (var order in source)
            {
                var brandId = await _services.GetBrandIdByPlayerCodeAsync(order.UserId).ConfigureAwait(false);
                var gameProviderConfigurationId = await _services.GetGameProviderConfigurationIdAsync(brandId, data.GameProviderId).ConfigureAwait(false);

                var settle = await _services.GetGameActionExternalIdAsync(data.GameProviderId, order.UserId, $"{ order.PlatformTxId}_settle").ConfigureAwait(false);
                if (settle != null)
                {
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = order.UserId,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        ExternalGameId = order.GameCode,
                        ExternalBetId = settle.ExternalBetId.ToString(),
                        ExternalRoundId = settle.ExternalRoundId.ToString(),
                        ExternalTransactionId = $"{ settle.ExternalTransactionId}_cancel",
                        ExternalTransactionReferenceId = settle.ExternalTransactionId,
                        GameActionType = GameActionType.CancelTransaction,
                        RoundClosed = false
                    });
                }
            }

            return GetBetData(data, batchTransactionData.ToArray());
        }

        async Task<BatchBetData> ISexyGamingActionModelMapper.VoidBetAsync(GameProviderConfigurationData data, SexyGamingVoidBett[] source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            foreach (var order in source)
            {
                var brandId = await _services.GetBrandIdByPlayerCodeAsync(order.UserId).ConfigureAwait(false);
                var gameProviderConfigurationId = await _services.GetGameProviderConfigurationIdAsync(brandId, data.GameProviderId).ConfigureAwait(false);
                var bet = await _services.GetGameActionExternalIdAsync(data.GameProviderId, order.UserId, $"{ order.PlatformTxId}").ConfigureAwait(false);
                if (bet != null)
                {
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = order.UserId,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        ExternalGameId = order.GameCode,
                        ExternalBetId = order.PlatformTxId.ToString(),
                        ExternalRoundId = order.RoundId.ToString(),
                        ExternalTransactionId = $"{ bet.ExternalTransactionId}_cancel_void_bet",
                        ExternalTransactionReferenceId = bet.ExternalTransactionId,
                        GameActionType = GameActionType.CancelTransaction,
                        Timestamp = DateTimeOffset.UtcNow
                    });
                }
            }

            return GetBetData(data, batchTransactionData.ToArray());
        }

        async Task<BatchBetData> ISexyGamingActionModelMapper.VoidSettleAsync(GameProviderConfigurationData data, string playCode, SexyGamingVoidSettle[] source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            foreach (var order in source)
            {
                var brandId = await _services.GetBrandIdByPlayerCodeAsync(order.UserId).ConfigureAwait(false);
                var gameProviderConfigurationId = await _services.GetGameProviderConfigurationIdAsync(brandId, data.GameProviderId).ConfigureAwait(false);
                var settle = await _services.GetGameActionExternalIdAsync(data.GameProviderId, order.UserId, $"{ order.PlatformTxId}_settle").ConfigureAwait(false);
                if (settle != null)
                {
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = order.UserId,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        ExternalGameId = order.GameCode,
                        ExternalBetId = order.PlatformTxId.ToString(),
                        ExternalRoundId = order.RoundId.ToString(),
                        ExternalTransactionId = $"{ settle.ExternalTransactionId}_cancel_void_settle",
                        ExternalTransactionReferenceId = settle.ExternalTransactionId,
                        GameActionType = GameActionType.CancelTransaction,
                        RoundClosed = false,
                        Timestamp = DateTimeOffset.UtcNow
                    });
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = order.UserId,
                        GameProviderConfigurationId = gameProviderConfigurationId,
                        ExternalGameId = order.GameCode,
                        ExternalBetId = order.PlatformTxId.ToString(),
                        ExternalRoundId = order.RoundId.ToString(),
                        ExternalTransactionId = $"{ settle.ExternalReferencedTransactionId}_cancel_void_bet",
                        ExternalTransactionReferenceId = settle.ExternalReferencedTransactionId,
                        GameActionType = GameActionType.CancelTransaction,
                        RoundClosed = true,
                        Timestamp = DateTimeOffset.UtcNow
                    });
                }
            }

            return GetBetData(data, batchTransactionData.ToArray());
        }

        private BetData GetBetData(GameProviderConfigurationData data, string playerCode,
          TransactionData[] transactions, bool allowPlaceBetWithDifferentPlatformToken = false,
          OppositeBetSetting[] oppositeBetSettings = null) =>
          new()
          {
              PlayerCode = playerCode,
              PlatformType = data.PlatformType,
              GameProviderId = data.GameProviderId,
              GameProviderConfigurationId = data.GameProviderConfigurationId,
              GameProviderCode = GameProviderCode,
              HasCurrencyFromGameProvider = false,
              AutoCloseRound = data.AutoCloseRound,
              AllowCancelCancelTransaction = false,
              AllowPlaceBetWithDifferentPlatformToken = allowPlaceBetWithDifferentPlatformToken,
              Transactions = transactions,
              OppositeBetRules = oppositeBetSettings?.Select(o => new OppositeBetRule
              {
                  ExternalGameIds = o.Games,
                  Patterns = o.Patterns?.Select(p => new OppositeBetRule.Pattern(p.BetType, p.GroupId)).ToArray()
              }).ToArray()
          };

        private BatchBetData GetBetData(GameProviderConfigurationData data, BatchTransactionData[] transactions)
            => new()
            {
                GameProviderId = data.GameProviderId,
                GameProviderCode = GameProviderCode,
                AutoCloseRound = data.AutoCloseRound,
                ExternalBatchId = string.Empty,
                AllowCancelCancelTransaction = false,
                HasCurrencyFromGameProvider = false,
                Transactions = transactions,
            };

        private GameActionType ProcessingSettlement(SexyGamingSettle source)
        {
            if (source.Platform.IgnoreCaseEquals("SEXYBCRT") && source.GameType.IgnoreCaseEquals("LIVE"))
            {
                var gameInfo = _jsonSerializationProvider.DeserializeFromString<SexyGamingGameRecord>(_jsonSerializationProvider.SerializeToString(source.GameInfo));
                return GetActionType(gameInfo.Status);
            }
            else
            {
                return GetActionType(source.WinAmount, source.BetAmount);
            }

        }

        private GameActionType GetActionType(decimal settleAmount, decimal betamount)
        {
            return (settleAmount == Math.Abs(betamount)) ? GameActionType.TieBet : (settleAmount > 0) ? GameActionType.WinBet : GameActionType.LoseBet;
        }

        private GameActionType GetActionType(string status) => status.ToLower() switch
        {
            "win" => GameActionType.WinBet,
            "lose" => GameActionType.LoseBet,
            "tie" => GameActionType.TieBet,
            _ => GameActionType.CancelTransaction
        };
    }
}
