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
using TGS.Shared.Game.Models.SexyGaming.Incoming;
using TGS.Shared.Game.Models.SexyGaming.Incoming.Transactions;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public interface ISexyGamingActionModelMapper
    {
        BetData BetAsync(GameProviderConfigurationData data, string playCode, SexyGamingBet[] source);

        Task<BetData> CancelBetAsync(GameProviderConfigurationData data, string playCode, SexyGamingCancelBet[] source);

        BatchBetData VoidBetAsync(GameProviderConfigurationData data, SexyGamingVoidBett[] source);

        BatchBetData SettleAsync(GameProviderConfigurationData data, SexyGamingSettle[] source, OppositeBetSetting[] oppositeBetSettings);

        Task<BatchBetData> UnsettleAsync(GameProviderConfigurationData data, SexyGamingUnsettle[] source);

        BatchBetData VoidSettleAsync(GameProviderConfigurationData data, SexyGamingVoidSettle[] source);
    }

    public class SexyGamingActionModelMapper : ISexyGamingActionModelMapper
    {
        private const string GameProviderCode = Consts.GameProvider.SexyGaming;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ISexyGamingServices _services;

        public SexyGamingActionModelMapper(
            ISexyGamingServices services,
            IJsonSerializationProvider jsonSerializationProvider
            )
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }


        BetData ISexyGamingActionModelMapper.BetAsync(GameProviderConfigurationData data, string playCode, SexyGamingBet[] source)
        {
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
            var transactionDatas = new List<TransactionData>();
            foreach (var order in source)
            {
                var bet = await _services.GetGameActionExternalIdAsync(data.GameProviderId, order.UserId, $"{ order.PlatformTxId.ToString()}").ConfigureAwait(false);
                if (bet != null)
                {
                    transactionDatas.Add(new TransactionData()
                    {
                        Amount = 0,
                        Commission = 0,
                        ExternalGameId = order.GameCode,
                        ExternalBetId = bet.ExternalBetId,
                        ExternalRoundId = bet.ExternalRoundId,
                        ExternalTransactionId = $"{ order.PlatformTxId}_cancelBet",
                        ExternalTransactionReferenceId = order.PlatformTxId,
                        GameActionType = GameActionType.CancelTransaction,
                        TimeStamp = DateTimeOffset.UtcNow,
                        RoundDetail = _jsonSerializationProvider.SerializeToString(order.GameInfo)
                    });
                }
            }

            return GetBetData(data, playCode, transactionDatas.ToArray());
        }

        BatchBetData ISexyGamingActionModelMapper.SettleAsync(GameProviderConfigurationData data, SexyGamingSettle[] source, OppositeBetSetting[] oppositeBetSettings)
        {
            var batchTransactionData = (from order in source
                                        select new BatchTransactionData()
                                        {
                                            PlayerCode = order.UserId,
                                            GameProviderConfigurationId = data.GameProviderConfigurationId,
                                            Amount = Math.Abs(order.WinAmount),
                                            Commission = 0,
                                            ExternalGameId = order.GameCode,
                                            ExternalBetId = order.PlatformTxId.ToString(),
                                            ExternalRoundId = order.RoundId.ToString(),
                                            ExternalTransactionId = $"{ order.PlatformTxId}_settle",
                                            ExternalTransactionReferenceId = order.PlatformTxId,
                                            GameActionType = (order.WinAmount == Math.Abs(order.BetAmount)) ? GameActionType.TieBet : (order.WinAmount > 0) ? GameActionType.WinBet : GameActionType.LoseBet,
                                            BetType = order.BetType,
                                            Timestamp = DateTime.UtcNow,
                                            RoundDetail = _jsonSerializationProvider.SerializeToString(order.GameInfo)
                                        }).ToArray();
            return GetBetData(data, batchTransactionData);
        }

        async Task<BatchBetData> ISexyGamingActionModelMapper.UnsettleAsync(GameProviderConfigurationData data, SexyGamingUnsettle[] source)
        {
            var batchTransactionData = new List<BatchTransactionData>();
            foreach (var order in source)
            {
                var settle = await _services.GetGameActionExternalIdAsync(data.GameProviderId, order.UserId, $"{ order.PlatformTxId}_settle").ConfigureAwait(false);
                if (settle != null)
                {
                    batchTransactionData.Add(new BatchTransactionData()
                    {
                        PlayerCode = order.UserId,
                        GameProviderConfigurationId = data.GameProviderConfigurationId,
                        Amount = 0,
                        Commission = 0,
                        ExternalGameId = order.GameCode,
                        ExternalBetId = settle.ExternalRoundId.ToString(),
                        ExternalRoundId = settle.ExternalRoundId.ToString(),
                        ExternalTransactionId = $"{ order.PlatformTxId}_unsettle",
                        ExternalTransactionReferenceId = order.PlatformTxId,
                        GameActionType = GameActionType.CancelTransaction,
                    });
                }
            }

            return GetBetData(data, batchTransactionData.ToArray());
        }

        BatchBetData ISexyGamingActionModelMapper.VoidBetAsync(GameProviderConfigurationData data, SexyGamingVoidBett[] source)
        {

            var batchTransactionData = (from order in source
                                        select new BatchTransactionData()
                                        {
                                            PlayerCode = order.UserId,
                                            GameProviderConfigurationId = data.GameProviderConfigurationId,
                                            Amount = 0,
                                            Commission = 0,
                                            ExternalGameId = order.GameCode,
                                            ExternalBetId = order.PlatformTxId.ToString(),
                                            ExternalRoundId = order.RoundId.ToString(),
                                            ExternalTransactionId = $"{ order.PlatformTxId.ToString()}_voidBet",
                                            ExternalTransactionReferenceId = order.PlatformTxId,
                                            GameActionType = GameActionType.CancelTransaction,
                                        }).ToArray();
            return GetBetData(data, batchTransactionData);
        }

        BatchBetData ISexyGamingActionModelMapper.VoidSettleAsync(GameProviderConfigurationData data, SexyGamingVoidSettle[] source)
        {
            var batchTransactionData = (from order in source
                                        select new BatchTransactionData()
                                        {
                                            PlayerCode = order.UserId,
                                            GameProviderConfigurationId = data.GameProviderConfigurationId,
                                            Amount = 0,
                                            Commission = 0,
                                            ExternalGameId = order.GameCode,
                                            ExternalBetId = order.PlatformTxId.ToString(),
                                            ExternalRoundId = order.RoundId.ToString(),
                                            ExternalTransactionId = $"{ order.PlatformTxId.ToString()}_voidSettle",
                                            ExternalTransactionReferenceId = order.PlatformTxId,
                                            GameActionType = GameActionType.CancelTransaction,
                                        }).ToArray();
            return GetBetData(data, batchTransactionData);
        }

        private static BetData GetBetData(GameProviderConfigurationData data, string playerCode,
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

        private static BatchBetData GetBetData(GameProviderConfigurationData data,
         BatchTransactionData[] transactions, bool allowPlaceBetWithDifferentPlatformToken = false,
         OppositeBetSetting[] oppositeBetSettings = null) =>
         new()
         {
             GameProviderId = data.GameProviderId,
             GameProviderCode = GameProviderCode,
             AutoCloseRound = false,
             ExternalBatchId = string.Empty,
             AllowCancelCancelTransaction = false,
             HasCurrencyFromGameProvider = false,
             Transactions = transactions,
         };
    }
}
