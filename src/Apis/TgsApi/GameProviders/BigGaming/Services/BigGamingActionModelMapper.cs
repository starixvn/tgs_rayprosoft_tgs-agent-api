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
        Task<(BetData SettleBetData, BetData CancelBetData)> SettleAsync(GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode);
        Task<(BetData BeforeCancel, BetData SettleBetData, BetData CancelBetData)> ResettleAsync(GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode);
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

        async Task<(BetData SettleBetData, BetData CancelBetData)> IBigGamingModelActionMapper.SettleAsync(GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode)
        {
            var settleTransactionData = new List<TransactionData>();
            var cancelTransactionData = new List<TransactionData>();
            foreach (var order in request.Orders)
            {
                var gameActionType = GetGameAction(order.OrderStatus);

                if (gameActionType != GameActionType.CancelTransaction)
                {
                    settleTransactionData.Add(await GetTransactionDataAsync(data.GameProviderId, playerCode, order).ConfigureAwait(false));
                }
                else
                {
                    cancelTransactionData.Add(await GetTransactionDataAsync(data.GameProviderId, playerCode, order).ConfigureAwait(false));
                }
            }

            return new(GetBetData(data, playerCode, settleTransactionData.ToArray()), GetBetData(data, playerCode, cancelTransactionData.ToArray()));
        }

        async Task<(BetData BeforeCancel, BetData SettleBetData, BetData CancelBetData)> IBigGamingModelActionMapper.ResettleAsync(GameProviderConfigurationData data, BigGamingSettleRequest request, string playerCode)
        {
            var beforeCancelTransactionData = new List<TransactionData>();
            var settleTransactionData = new List<TransactionData>();
            var cancelTransactionData = new List<TransactionData>();
            foreach (var order in request.Orders)
            {

                var gameActionExternalIds = (await _services.GetGameActionExternalIdAsync(data.GameProviderId, playerCode, $"{order.OrderId}_settle").ConfigureAwait(false));

                if (gameActionExternalIds != null)
                {
                    beforeCancelTransactionData.Add(new TransactionData
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
                    settleTransactionData.Add(await GetTransactionDataAsync(data.GameProviderId, playerCode, order, true).ConfigureAwait(false));
                }
                else
                {
                    cancelTransactionData.Add(await GetTransactionDataAsync(data.GameProviderId, playerCode, order, true).ConfigureAwait(false));
                }
            }
            return new(
                GetBetData(data, playerCode, beforeCancelTransactionData.ToArray()),
                GetBetData(data, playerCode, cancelTransactionData.ToArray()),
                GetBetData(data, playerCode, settleTransactionData.ToArray())
                );
        }

        #region Private Methods

        private async Task<TransactionData> GetTransactionDataAsync(Guid gameProviderId, string playerCode, BigGamingSettleOrderRequest order, bool Resettle = false)
        {
            var gameActionExternal = (await _services.GetGameActionExternalIdAsync(gameProviderId, playerCode, order.OrderId.ToString()).ConfigureAwait(false));
            var gameActionType = GetGameAction(order.OrderStatus);

            var mark = (!Resettle) ? (gameActionType != GameActionType.CancelTransaction) ? "settle" : "cancel" : "resettle";

            if (gameActionType != GameActionType.CancelTransaction)
            {
                return new TransactionData
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
            else
            {
                return new TransactionData
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
        }

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
