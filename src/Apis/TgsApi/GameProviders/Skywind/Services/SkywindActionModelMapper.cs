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
        BetData Bet(GameProviderConfigurationData data, string playCode, SkywindBetRequest[] source);

        BetData SettleAsync(GameProviderConfigurationData data, string playCode, SkywindSettleRequest[] source);

        Task<BetData> RollbackBetAsync(GameProviderConfigurationData data, string playCode, SkywindRollbackBetRequest[] source);
    }

    public class SkywindActionModelMapper : ISkywindActionModelMapper
    {
        private const string _gameProviderCode = Consts.GameProvider.Skywind;

        private readonly ISkywindServices _services;

        public SkywindActionModelMapper(ISkywindServices services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }


        BetData ISkywindActionModelMapper.Bet(GameProviderConfigurationData data, string playCode, SkywindBetRequest[] source)
        {

            var transactions = (from order in source
                                select new TransactionData
                                {
                                    Amount = Math.Abs(order.Amount),
                                    ExternalTransactionId = order.TrxId,
                                    ExternalRoundId = _services.GetExternalRoundId(order.RoundId, order.EventId),
                                    ExternalBetId = order.TrxId.ToString(),
                                    ExternalGameActionGroupId = order.TrxId.ToString(),
                                    ExternalGameId = order.GameCode.ToString(),
                                    GameActionType = GameActionType.PlaceBet,
                                    ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(order.Timestamp)).ToIso8601String(true),
                                    TimeStamp = DateTimeOffset.UtcNow,
                                }).ToArray();
            return GetBetData(data, playCode, transactions);
        }

        BetData ISkywindActionModelMapper.SettleAsync(GameProviderConfigurationData data, string playCode, SkywindSettleRequest[] source)
        {
            var transactions = (from order in source
                                select new TransactionData
                                {
                                    Amount = Math.Abs(order.Amount),
                                    ExternalTransactionId = $"{order.TrxId}_settle",
                                    ExternalRoundId = _services.GetExternalRoundId(order.RoundId, order.EventId),
                                    ExternalBetId = order.TrxId.ToString(),
                                    ExternalGameActionGroupId = order.TrxId.ToString(),
                                    ExternalGameId = order.GameCode.ToString(),
                                    ExternalTransactionReferenceId = order.TrxId.ToString(),
                                    GameActionType = ProcessingSettlement(order),
                                    ExtraData = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(order.Timestamp)).ToIso8601String(true),
                                    TimeStamp = DateTimeOffset.UtcNow,
                                }).ToArray();

            return GetBetData(data, playCode, transactions);
        }

        async Task<BetData> ISkywindActionModelMapper.RollbackBetAsync(GameProviderConfigurationData data, string playCode, SkywindRollbackBetRequest[] source)
        {
            var transactions = new List<TransactionData>();
            foreach (var order in source)
            {

                var bet = await _services.GetGameActionExternalIdAsync(data.GameProviderId, order.CustId, $"{ order.TrxId}").ConfigureAwait(false);
                if (bet != null)
                {
                    transactions.Add(new TransactionData()
                    {

                        ExternalGameId = order.GameCode,
                        ExternalBetId = bet.ExternalBetId.ToString(),
                        ExternalRoundId = bet.ExternalRoundId.ToString(),
                        ExternalTransactionId = $"{ bet.ExternalTransactionId}_cancel_bet",
                        ExternalTransactionReferenceId = bet.ExternalTransactionId,
                        GameActionType = GameActionType.CancelTransaction,
                        RoundClosed = true
                    });
                }
            }

            return GetBetData(data, playCode, transactions.ToArray());
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

        private GameActionType ProcessingSettlement(SkywindSettleRequest source)
        {
            if (source.GameType.IgnoreCaseEquals("freegame") && source.GameType.IgnoreCaseEquals("bonusgame"))
            {
                if (source.GameType.IgnoreCaseEquals("freegame"))
                {
                    return GameActionType.FreeBet;
                }
                else
                {
                    return GameActionType.WinJackpot;

                }
            }
            else
            {
                if (source.Amount > 0)
                {
                    return GameActionType.WinBet;
                }
                else
                {
                    return GameActionType.LoseBet;
                }
            }
        }

    }
}
