using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Skywind.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public interface ISkywindActionModelMapper
    {
        BetData BetAsync(GameProviderConfigurationData data, string playCode, SkywindBetRequest source);

        BetData SettleAsync(GameProviderConfigurationData data, string playCode, SkywindSettleRequest[] source);
    }

    public class SkywindActionModelMapper : ISkywindActionModelMapper
    {
        private const string _gameProviderCode = Consts.GameProvider.Skywind;

        BetData ISkywindActionModelMapper.BetAsync(GameProviderConfigurationData data, string playCode, SkywindBetRequest source)
        {

            var transactions = new TransactionData()
            {

                Amount = Math.Abs(source.Amount),
                ExternalTransactionId = source.EventId.ToString(),
                ExternalRoundId = source.RoundId,
                ExternalBetId = source.TrxId.ToString(),
                ExternalGameActionGroupId = source.TrxId.ToString(),
                ExternalGameId = source.GameCode.ToString(),
                GameActionType = GameActionType.PlaceBet,
                ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(source.Timestamp)).ToIso8601String(true),
                TimeStamp = DateTimeOffset.UtcNow,
                //BetType = order.BetType
            };
            return GetBetData(data, playCode, new[] { transactions });
        }

        BetData ISkywindActionModelMapper.SettleAsync(GameProviderConfigurationData data, string playCode, SkywindSettleRequest[] source)
        {
            var transactions = (from order in source
                                select new TransactionData
                                {
                                    Amount = Math.Abs(order.Amount),
                                    ExternalTransactionId = order.EventId.ToString(),
                                    ExternalRoundId = order.RoundId,
                                    ExternalBetId = order.TrxId.ToString(),
                                    ExternalGameActionGroupId = order.TrxId.ToString(),
                                    ExternalGameId = order.GameCode.ToString(),
                                    GameActionType = GameActionType.PlaceBet,
                                    ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(order.Timestamp)).ToIso8601String(true),
                                    TimeStamp = DateTimeOffset.UtcNow,
                                    //BetType = order.BetType
                                }).ToArray();

            return GetBetData(data, playCode, transactions);
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
               GameProviderCode = _gameProviderCode,
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
