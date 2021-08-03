using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.BigGaming.Enums;
using TGS.Shared.Game.Models.BigGaming.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public interface IBigGamingModelActionMapper
    {
        BetData Request(GameProviderConfigurationData data, BigGamingBetRequest request,
            OppositeBetSetting[] oppositeBetSettings);

        ValueTask<(BetData SettleBetData, BetData CancelBetData)> SettleAsync(GameProviderConfigurationData data,
            BigGamingSettleRequest request, string playerCode, OppositeBetSetting[] oppositeBetSettings);

        ValueTask<(BetData BeforeCancel, BetData SettleBetData, BetData CancelBetData)> ResettleAsync(
            GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode,
            OppositeBetSetting[] oppositeBetSettings);
    }

    public class BigGamingActionModelMapper : IBigGamingModelActionMapper
    {
        private readonly IBigGamingService _service;
        private readonly Dictionary<string, decimal> _bullBullOddsMapper;

        public BigGamingActionModelMapper(
            IBigGamingService service
        )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _bullBullOddsMapper = new Dictionary<string, decimal>() {
                {"0xD0000001",1 } //下注闲1平倍
                ,{"0xD0000002",5 }//下注闲1翻倍
                ,{"0xD0000004",1 }//下注闲2平倍
                ,{"0xD0000008",5 }//下注闲2翻倍
                ,{"0xD0000010",1 }//下注闲3平倍
                ,{"0xD0000020",5 }//下注闲3翻倍
            };
        }


        BetData IBigGamingModelActionMapper.Request(GameProviderConfigurationData data, BigGamingBetRequest request,
            OppositeBetSetting[] oppositeBetSettings)
        {
            var transactions = (from order in request.Orders
                                select new TransactionData
                                {
                                    Amount = GetAmount(order.ExternalGameId, GetBetType(order.PlayId), Math.Abs(order.Amount)),
                                    ExternalTransactionId = order.OrderId.ToString(),
                                    ExternalRoundId = order.IssueId,
                                    ExternalBetId = order.OrderId.ToString(),
                                    ExternalGameActionGroupId = order.OrderId.ToString(),
                                    ExternalGameId = order.ExternalGameId.ToString(),
                                    GameActionType = GameActionType.PlaceBet,
                                    TimeStamp = DateTimeOffset.UtcNow,
                                    BetType = GetBetType(order.PlayId)
                                }).ToArray();

            return GetBetData(data, request.LoginId, transactions, oppositeBetSettings: oppositeBetSettings);
        }

        async ValueTask<(BetData SettleBetData, BetData CancelBetData)> IBigGamingModelActionMapper.SettleAsync(
            GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode,
            OppositeBetSetting[] oppositeBetSettings)
        {
            var settleTransactionData = new List<TransactionData>();
            var cancelTransactionData = new List<TransactionData>();

            foreach (var order in request.Orders)
            {
                var gameActionType = GetGameAction(order.OrderStatus);

                if (gameActionType != GameActionType.CancelTransaction)
                {
                    settleTransactionData.Add(await GetTransactionDataAsync(data.GameProviderId, playerCode, order)
                        .ConfigureAwait(false));
                }
                else
                {
                    cancelTransactionData.Add(await GetTransactionDataAsync(data.GameProviderId, playerCode, order)
                        .ConfigureAwait(false));
                }
            }

            return new(
                GetBetData(data, playerCode, settleTransactionData.ToArray(), oppositeBetSettings: oppositeBetSettings),
                GetBetData(data, playerCode, cancelTransactionData.ToArray(),
                    oppositeBetSettings: oppositeBetSettings));
        }

        async ValueTask<(BetData BeforeCancel, BetData SettleBetData, BetData CancelBetData)> IBigGamingModelActionMapper.
            ResettleAsync(GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode,
                OppositeBetSetting[] oppositeBetSettings)
        {
            var beforeCancelTransactionData = new List<TransactionData>();
            var settleTransactionData = new List<TransactionData>();
            var cancelTransactionData = new List<TransactionData>();

            foreach (var order in request.Orders)
            {
                var gameActionExternalIds = await _service
                    .GetGameActionExternalIdAsync(data.GameProviderId, playerCode, $"{order.OrderId}_settle")
                    .ConfigureAwait(false);

                if (gameActionExternalIds != null)
                {
                    beforeCancelTransactionData.Add(new()
                    {
                        Amount = Math.Abs(gameActionExternalIds.Amount),
                        ExternalTransactionId = $"{gameActionExternalIds.ExternalTransactionId}_cancel",
                        ExternalRoundId = gameActionExternalIds.ExternalRoundId,
                        ExternalBetId = order.OrderId.ToString(),
                        ExternalGameActionGroupId = order.OrderId.ToString(),
                        ExternalGameId = order.ExternalGameId.ToString(),
                        GameActionType = GameActionType.CancelTransaction,
                        TimeStamp = DateTimeOffset.UtcNow,
                        ExternalTransactionReferenceId = gameActionExternalIds.ExternalTransactionId,
                        RoundClosed = false
                    });
                }


                var gameActionType = GetGameAction(order.OrderStatus);

                if (gameActionType != GameActionType.CancelTransaction)
                {
                    settleTransactionData.Add(
                        await GetTransactionDataAsync(data.GameProviderId, playerCode, order, true)
                            .ConfigureAwait(false));
                }
                else
                {
                    cancelTransactionData.Add(
                        await GetTransactionDataAsync(data.GameProviderId, playerCode, order, true)
                            .ConfigureAwait(false));
                }
            }

            return new(
                GetBetData(data, playerCode, beforeCancelTransactionData.ToArray(),
                    oppositeBetSettings: oppositeBetSettings),
                GetBetData(data, playerCode, cancelTransactionData.ToArray(),
                    oppositeBetSettings: oppositeBetSettings),
                GetBetData(data, playerCode, settleTransactionData.ToArray(),
                    oppositeBetSettings: oppositeBetSettings)
            );
        }

        #region Private Methods

        private async Task<TransactionData> GetTransactionDataAsync(Guid gameProviderId, string playerCode,
            BigGamingSettleOrderRequest order, bool resettle = false)
        {
            var gameActionExternal = await _service
                .GetGameActionExternalIdAsync(gameProviderId, playerCode, order.OrderId.ToString())
                .ConfigureAwait(false);

            var gameActionType = GetGameAction(order.OrderStatus);

            //牛牛特殊案例
            if (order.Amount > 0 && gameActionExternal.ExternalGameId == "13")
            {
                gameActionType = GameActionType.WinBet;
            }

            var mark = !resettle
                ? gameActionType != GameActionType.CancelTransaction ? "settle" : "cancel"
                : "resettle";

            if (gameActionType != GameActionType.CancelTransaction)
            {
                return new()
                {
                    Amount = Math.Abs(order.Amount),
                    ExternalTransactionId = $"{order.OrderId}_{mark}",
                    ExternalRoundId = gameActionExternal.ExternalRoundId,
                    ExternalBetId = order.OrderId.ToString(),
                    ExternalGameActionGroupId = order.OrderId.ToString(),
                    ExternalGameId = order.ExternalGameId.ToString(),
                    GameActionType = gameActionType,
                    TimeStamp = DateTimeOffset.UtcNow,
                    ExternalTransactionReferenceId = order.OrderId.ToString()
                };
            }

            return new()
            {
                Amount = Math.Abs(order.Amount),
                ExternalTransactionId = $"{order.OrderId}_{mark}",
                ExternalRoundId = gameActionExternal.ExternalRoundId,
                ExternalBetId = order.OrderId.ToString(),
                ExternalGameActionGroupId = order.OrderId.ToString(),
                ExternalGameId = order.ExternalGameId.ToString(),
                GameActionType = gameActionType,
                TimeStamp = DateTimeOffset.UtcNow,
                ExternalTransactionReferenceId = order.OrderId.ToString()
            };
        }

        private GameActionType GetGameAction(BigGamingOrderStatus betStatus) => betStatus switch
        {
            BigGamingOrderStatus.Running => GameActionType.PlaceBet,
            BigGamingOrderStatus.Win => GameActionType.WinBet,
            BigGamingOrderStatus.Tie => GameActionType.TieBet,
            BigGamingOrderStatus.Lose => GameActionType.LoseBet,
            _ => GameActionType.CancelTransaction
        };

        private static BetData GetBetData(GameProviderConfigurationData data, string playerCode,
            TransactionData[] transactions, bool allowPlaceBetWithDifferentPlatformToken = false,
            OppositeBetSetting[] oppositeBetSettings = null) =>
            new()
            {
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                GameProviderCode = Consts.GameProvider.BigGaming,
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

        private decimal GetAmount(int externalGameId, string playId, decimal amount)
        {
            if (externalGameId == 13)
            {
                var Odds = _bullBullOddsMapper.Where(x => x.Key.IgnoreCaseEquals(playId)).First().Value;
                return amount * Odds;
            }
            return amount;
        }


        private string GetBetType(string playId) =>
            $"0x{Convert.ToString(Convert.ToInt64(playId), 16).ToUpper()}";
        #endregion
    }
}