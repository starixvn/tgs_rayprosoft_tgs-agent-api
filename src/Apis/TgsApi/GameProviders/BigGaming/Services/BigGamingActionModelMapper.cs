using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.BigGaming.Enums;
using TGS.Shared.Game.Models.BigGaming.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public interface IBigGamingModelActionMapper
    {
        BetData Request(GameProviderConfigurationData data, BigGamingBetRequest request);
        Task<decimal> SettleAsync(GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode);
        Task<decimal> ResettleAsync(GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode);
    }
    public class BigGamingActionModelMapper : IBigGamingModelActionMapper
    {
        private const string GameProviderCode = Consts.GameProvider.BigGaming;
        private readonly IBigGamingServices _services;
        private readonly IGamePlayActions _actions;
        private readonly IGameProviderOperations _gameProviderOperations;
        public BigGamingActionModelMapper(
            IBigGamingServices services,
             IGamePlayActions actions,
            IGameProviderOperations gameProviderOperations
            )
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        BetData IBigGamingModelActionMapper.Request(GameProviderConfigurationData data, BigGamingBetRequest request)
        {
            var transactions = (from Order in request.Orders
                                select new TransactionData
                                {
                                    Amount = Math.Abs(Order.Amount),
                                    ExternalTransactionId = Order.OrderId.ToString(),
                                    ExternalRoundId = Order.IssueId,
                                    ExternalBetId = Order.OrderId.ToString(),
                                    ExternalGameActionGroupId = Order.OrderId.ToString(),
                                    ExternalGameId = Order.ExternalGameId.ToString(),
                                    GameActionType = GameActionType.PlaceBet,
                                    TimeStamp = DateTimeOffset.UtcNow
                                }).ToArray();
            return GetBetData(data, request.LoginId, transactions);
        }

        async Task<decimal> IBigGamingModelActionMapper.SettleAsync(GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode)
        {
            var balance = default(decimal);
            foreach (var order in request.Orders)
            {
                var gameActionExternal = (await _services.GetGameActionExternalIdAsync(data.GameProviderId, playerCode, order.OrderId.ToString()).ConfigureAwait(false));
                var gameActionType = GetGameAction(order.OrderStatus);
                var mark = (gameActionType != GameActionType.CancelTransaction) ? "settle" : "cancel";
                var transactions = new TransactionData[]
                {
                new TransactionData{
                Amount = Math.Abs(order.Amount),
                ExternalTransactionId = $"{order.OrderId}_{mark}",
                ExternalRoundId = gameActionExternal.ExternalRoundId,
                ExternalBetId = order.OrderId.ToString(),
                ExternalGameActionGroupId = order.OrderId.ToString(),
                ExternalGameId = order.ExternalGameId.ToString(),
                GameActionType =gameActionType,
                TimeStamp = DateTimeOffset.UtcNow,
                ExternalTransactionReferenceId= order.OrderId.ToString(),
                }
                };

                var betData = GetBetData(data, playerCode, transactions);

                var betResult = default(BetResult);
                if (gameActionType != GameActionType.CancelTransaction)
                {
                    betResult = await _actions.SettleBetAsync(betData).ConfigureAwait(false);
                }
                else
                {
                    betResult = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
                }

                balance = betResult.Balance;
            }

            return balance;
        }

        async Task<decimal> IBigGamingModelActionMapper.ResettleAsync(GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode)
        {
            var balance = default(decimal);
            foreach (var order in request.Orders)
            {

                var gameActionExternalIds = (await _services.GetGameActionExternalIdAsync(data.GameProviderId, playerCode, $"{order.OrderId}_settle").ConfigureAwait(false));

                if (gameActionExternalIds != null)
                {
                    var transactionsCancel = new TransactionData[]
                    {
                        new TransactionData{
                            Amount = Math.Abs(gameActionExternalIds.Amount),
                            ExternalTransactionId = $"{gameActionExternalIds.ExternalTransactionId}_cancel",
                            ExternalRoundId = gameActionExternalIds.ExternalRoundId,
                            ExternalBetId = order.OrderId.ToString(),
                            ExternalGameActionGroupId = order.OrderId.ToString(),
                            ExternalGameId = order.ExternalGameId.ToString(),
                            GameActionType =GameActionType.CancelTransaction,
                            TimeStamp = DateTimeOffset.UtcNow,
                            ExternalTransactionReferenceId=gameActionExternalIds.ExternalTransactionId
                        }
                    };
                    var betDataCancel = GetBetData(data, playerCode, transactionsCancel);
                    var betResultBetData = await _actions.CancelTransactionAsync(betDataCancel).ConfigureAwait(false);
                }

                var externalRoundId = (await _services.GetGameActionExternalIdAsync(data.GameProviderId, playerCode, order.OrderId.ToString()).ConfigureAwait(false)).ExternalRoundId;
                var gameActionType = GetGameAction(order.OrderStatus);

                var transactions = new TransactionData[]
                {
                    new TransactionData{
                        Amount = Math.Abs(order.Amount),
                        ExternalTransactionId = $"{order.OrderId}_resettle",
                        ExternalRoundId = externalRoundId,
                        ExternalBetId = order.OrderId.ToString(),
                        ExternalGameActionGroupId = order.OrderId.ToString(),
                        ExternalGameId = order.ExternalGameId.ToString(),
                        GameActionType =gameActionType,
                        TimeStamp = DateTimeOffset.UtcNow
                    }
                };

                var betData = GetBetData(data, playerCode, transactions);
                var betResult = default(BetResult);
                if (gameActionType != GameActionType.CancelTransaction)
                {
                    betResult = await _actions.SettleBetAsync(betData).ConfigureAwait(false);
                }
                else
                {
                    betResult = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
                }

                balance = betResult.Balance;
            }
            return balance;
        }



        #region Private Methods

        private GameActionType GetGameAction(BigGamingOrderStatus betStatus) => betStatus switch
        {
            BigGamingOrderStatus.Running => GameActionType.PlaceBet,
            BigGamingOrderStatus.Win => GameActionType.WinBet,
            BigGamingOrderStatus.Tie => GameActionType.TieBet,
            BigGamingOrderStatus.Lose => GameActionType.LoseBet,
            _ => GameActionType.CancelTransaction,
        };

        private static BetData GetBetData(GameProviderConfigurationData data, string playerCode,
            TransactionData[] transactions, bool allowPlaceBetWithDifferentPlatformToken = false) =>
            new()
            {
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                GameProviderCode = GameProviderCode,
                HasCurrencyFromGameProvider = false,
                AutoCloseRound = data.AutoCloseRound,
                AllowCancelCancelTransaction = true,
                AllowPlaceBetWithDifferentPlatformToken = allowPlaceBetWithDifferentPlatformToken,
                Transactions = transactions
            };
        #endregion
    }
}
