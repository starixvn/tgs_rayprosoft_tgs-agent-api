using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        BetData CancelBetAsync(GameProviderConfigurationData data, string playCode, SexyGamingCancelBet[] source);

        BetData VoidBetAsync(GameProviderConfigurationData data, string playCode, SexyGamingVoidBett[] source);

        BetData SettleAsync(GameProviderConfigurationData data, string playCode, SexyGamingSettle[] source, OppositeBetSetting[] oppositeBetSettings);

        BetData UnsettleAsync(GameProviderConfigurationData data, string playCode, SexyGamingUnsettle[] source, OppositeBetSetting[] oppositeBetSettings);

        BetData VoidSettleAsync(GameProviderConfigurationData data, string playCode, SexyGamingVoidSettle[] source, OppositeBetSetting[] oppositeBetSettings);
    }

    public class SexyGamingActionModelMapper : ISexyGamingActionModelMapper
    {
        private const string GameProviderCode = Consts.GameProvider.BigGaming;

        private readonly Dictionary<string, decimal> _bullBullOddsMapper;

        public SexyGamingActionModelMapper()
        { }


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
                                    TimeStamp = DateTimeOffset.UtcNow,
                                    BetType = order.BetType
                                }).ToArray();

            return GetBetData(data, playCode, transactions);
        }

        BetData ISexyGamingActionModelMapper.CancelBetAsync(GameProviderConfigurationData data, string playCode, SexyGamingCancelBet[] source)
        {
            var transactions = (from order in source
                                select new TransactionData
                                {
                                    Amount = 0,
                                    ExternalTransactionId = $"{ order.PlatformTxId.ToString()}_cancel",
                                    ExternalRoundId = order.RoundId,
                                    ExternalBetId = order.PlatformTxId.ToString(),
                                    ExternalGameActionGroupId = order.PlatformTxId.ToString(),
                                    ExternalGameId = order.GameCode.ToString(),
                                    GameActionType = GameActionType.CancelTransaction,
                                    TimeStamp = DateTimeOffset.UtcNow,
                                }).ToArray();

            return GetBetData(data, playCode, transactions);
        }

        BetData ISexyGamingActionModelMapper.SettleAsync(GameProviderConfigurationData data, string playCode, SexyGamingSettle[] source, OppositeBetSetting[] oppositeBetSettings)
        {
            var transactions = (from order in source
                                select new TransactionData
                                {
                                    Amount = Math.Abs(order.WinAmount),
                                    ExternalTransactionId = $"{ order.PlatformTxId.ToString()}_settle",
                                    ExternalBetId = order.PlatformTxId.ToString(),
                                    ExternalGameActionGroupId = order.PlatformTxId.ToString(),
                                    ExternalGameId = order.GameCode.ToString(),
                                    GameActionType = (order.WinAmount == Math.Abs(order.BetAmount)) ? GameActionType.TieBet : (order.WinAmount > 0) ? GameActionType.WinBet : GameActionType.LoseBet,
                                    TimeStamp = DateTimeOffset.UtcNow,
                                }).ToArray();

            return GetBetData(data, playCode, transactions, oppositeBetSettings: oppositeBetSettings);
        }

        BetData ISexyGamingActionModelMapper.UnsettleAsync(GameProviderConfigurationData data, string playCode, SexyGamingUnsettle[] source, OppositeBetSetting[] oppositeBetSettings)
        {
            var transactions = (from order in source
                                select new TransactionData
                                {
                                    Amount = 0,
                                    ExternalTransactionId = $"{ order.PlatformTxId.ToString()}_cancel",
                                    ExternalBetId = order.PlatformTxId.ToString(),
                                    ExternalGameActionGroupId = order.PlatformTxId.ToString(),
                                    ExternalGameId = order.GameCode.ToString(),
                                    GameActionType = GameActionType.CancelTransaction,
                                    TimeStamp = DateTimeOffset.UtcNow,
                                }).ToArray();

            return GetBetData(data, playCode, transactions, oppositeBetSettings: oppositeBetSettings);
        }

        BetData ISexyGamingActionModelMapper.VoidBetAsync(GameProviderConfigurationData data, string playCode, SexyGamingVoidBett[] source)
        {
            var transactions = (from order in source
                                select new TransactionData
                                {
                                    Amount = Math.Abs(order.BetAmount),
                                    ExternalTransactionId = $"{ order.PlatformTxId.ToString()}_cancel",
                                    ExternalBetId = order.PlatformTxId.ToString(),
                                    ExternalGameActionGroupId = order.PlatformTxId.ToString(),
                                    ExternalGameId = order.GameCode.ToString(),
                                    GameActionType = GameActionType.CancelTransaction,
                                    TimeStamp = DateTimeOffset.UtcNow,
                                }).ToArray();

            return GetBetData(data, playCode, transactions);
        }

        BetData ISexyGamingActionModelMapper.VoidSettleAsync(GameProviderConfigurationData data, string playCode, SexyGamingVoidSettle[] source, OppositeBetSetting[] oppositeBetSettings)
        {
            var transactions = (from order in source
                                select new TransactionData
                                {
                                    Amount = Math.Abs(order.BetAmount),
                                    ExternalTransactionId = $"{ order.PlatformTxId.ToString()}_cancel",
                                    ExternalBetId = order.PlatformTxId.ToString(),
                                    ExternalGameActionGroupId = order.PlatformTxId.ToString(),
                                    ExternalGameId = order.GameCode.ToString(),
                                    GameActionType = GameActionType.CancelTransaction,
                                    TimeStamp = DateTimeOffset.UtcNow,
                                }).ToArray();

            return GetBetData(data, playCode, transactions, oppositeBetSettings: oppositeBetSettings);
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
    }
}
