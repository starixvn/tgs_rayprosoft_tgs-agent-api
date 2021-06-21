// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.IMESports.Enums;
using TGS.Shared.Game.Models.IMESports.Incoming;
using static TGS.Shared.Game.Models.IMESports.Enums.ActionType;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.IMESports.Services
{
    public interface IIMESportsActionModelMapper
    {
        BetData Request(GameProviderConfigurationData data, BetRequest request, string playerCode,
            bool allowPlaceBetWithDifferentPlatformToken);

        GetBalanceResponse Response(string playerCode, GetBalanceResult r);

        BetResponse Response(BetResult r);

        (BatchBetData BatchBetData, IEnumerable<string> NotFoundTransactions) Request(Guid gameProviderId,
            Dictionary<string, (Guid GameProviderConfigurationId, string PlayerCode)> playerConfigurationIds,
            IDictionary<string, IEnumerable<GameActionExternalIds>> externalIds,
            BetRequest request, bool isSettle);

        IEnumerable<TransactionResponse> Response(BatchBetResult r);
    }

    public sealed class IMESportsActionModelMapper : IIMESportsActionModelMapper
    {
        private const string GameProviderCode = Consts.GameProvider.IMESports;
        private const bool HasCurrencyFromGameProvider = false;

        GetBalanceResponse IIMESportsActionModelMapper.Response(string playerCode, GetBalanceResult r) =>
            new()
            {
                PlayerCode = playerCode,
                Currency = r.GameProviderCurrencyCode,
                Balance = r.Balance
            };

        BetData IIMESportsActionModelMapper.Request(GameProviderConfigurationData data, BetRequest request,
            string playerCode, bool allowPlaceBetWithDifferentPlatformToken)
        {
            var transactions = request.Transactions.Select(t => new TransactionData
            {
                Amount = Math.Abs(t.Amount),
                ExternalTransactionId = t.ExternalTransactionId,
                ExternalRoundId = t.ExternalRoundId,
                ExternalBetId = t.ExternalRoundId,
                ExternalGameActionGroupId = t.ExternalRoundId,
                ExternalGameId = t.ExternalGameId,
                GameActionType = GameActionType.PlaceBet,
                TimeStamp = t.Timestamp
            }).ToArray();

            return GetBetData(data, playerCode, transactions, allowPlaceBetWithDifferentPlatformToken);
        }

        BetResponse IIMESportsActionModelMapper.Response(BetResult r)
            => new()
            {
                Results = (from res in r.Transactions
                           select new TransactionResponse
                           {
                               InternalTransactionId = res.InternalTransactionId!.Value,
                               ExternalTransactionId = res.ExternalTransactionId,
                               Balance = r.Balance
                           }).ToArray()
            };

        (BatchBetData BatchBetData, IEnumerable<string> NotFoundTransactions) IIMESportsActionModelMapper.Request(
            Guid gameProviderId,
            Dictionary<string, (Guid GameProviderConfigurationId, string PlayerCode)> playerConfigurationIds,
            IDictionary<string, IEnumerable<GameActionExternalIds>> externalList,
            BetRequest request, bool isSettle)
        {
            var notFoundTransactions = new List<string>();

            var transactions = request.Transactions
                .Where(t => GetGameActionType(t.ActionType, t.Amount).IsSettleBet() == isSettle).Select(t =>
                {
                    playerConfigurationIds.TryGetValue(t.PlayerCode, out var playerGameProviderConfigurationId);
                    var externalIds = externalList[t.ExternalRoundId];
                    var externalId = externalIds.FirstOrDefault(e =>
                        GetRefGameActionTypes(t.ActionType).Contains(e.GameActionType));

                    if (externalId != null)
                    {
                        return new BatchTransactionData
                        {
                            PlayerCode = playerGameProviderConfigurationId.PlayerCode,
                            GameProviderConfigurationId =
                                playerGameProviderConfigurationId.GameProviderConfigurationId,
                            Amount = Math.Abs(t.Amount),
                            ExternalRoundId = t.ExternalRoundId,
                            ExternalBetId = t.ExternalRoundId,
                            ExternalGameId = t.ExternalGameId,
                            ExternalTransactionId = t.ExternalTransactionId,
                            ExternalTransactionReferenceId = externalId.ExternalTransactionId,
                            GameActionType = GetGameActionType(t.ActionType, t.Amount),
                            RoundClosed = GetRoundClosedFlag(t.ActionType),
                            CurrencyCode = t.Currency,
                            Timestamp = t.Timestamp
                        };
                    }

                    notFoundTransactions.Add(t.ExternalTransactionId);
                    return null;
                }).ToArray();

            var batchBetData = new BatchBetData
            {
                ExternalBatchId = Guid.NewGuid().ToString(),
                GameProviderId = gameProviderId,
                GameProviderCode = GameProviderCode,
                AutoCloseRound = true,
                AllowCancelCancelTransaction = true,
                Transactions = transactions.Where(t => t != null).ToArray(),
                ErrorMapperKey = GameProviderCode
            };

            return (batchBetData, notFoundTransactions);
        }

        IEnumerable<TransactionResponse> IIMESportsActionModelMapper.Response(BatchBetResult r)
        {
            var results = new List<TransactionResponse>();

            results.AddRange(
                from result in r.Results
                join balance in r.PlayerBalances
                    on result.PlayerCode equals balance.Key
                select new TransactionResponse
                {
                    ExternalTransactionId = result.ExternalTransactionId,
                    InternalTransactionId = result.InternalTransactionId,
                    Balance = balance.Value
                });

            results.AddRange(
                from error in r.Errors
                select new TransactionResponse
                {
                    ExternalTransactionId = error.ExternalTransactionId,
                    ErrorCode = error.Error,
                    ErrorMessage = error.ErrorMessage
                });

            return results;
        }

        #region Private Methods

        private static BetData GetBetData(GameProviderConfigurationData data, string playerCode,
            TransactionData[] transactions, bool allowPlaceBetWithDifferentPlatformToken = false) =>
            new()
            {
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                GameProviderCode = GameProviderCode,
                HasCurrencyFromGameProvider = HasCurrencyFromGameProvider,
                AutoCloseRound = data.AutoCloseRound,
                AllowCancelCancelTransaction = true,
                AllowPlaceBetWithDifferentPlatformToken = allowPlaceBetWithDifferentPlatformToken,
                Transactions = transactions
            };

        private static GameActionType GetGameActionType(ActionType actionId, decimal amount) =>
            actionId switch
            {
                // settle
                SettleFT or SettleParlay => amount > 0 ? WinBet : LoseBet,

                // cancel placebet
                PlaceBetRefund or CancelFT or CancelSingleWager or CancelParlay => CancelTransaction,

                // cancel settlement
                UnsettleFT or UnsettleParlay => CancelTransaction,

                // cancel cancel
                UncancelFT or UncancelSingleWager or UncancelParlay => CancelTransaction,

                _ => throw new ArgumentOutOfRangeException(nameof(actionId),
                    $"unrecognized action type id: {actionId}")
            };

        private static GameActionType[] GetRefGameActionTypes(ActionType actionId) =>
            actionId switch
            {
                // settle
                SettleFT or SettleParlay => new[] {GameActionType.PlaceBet},

                // cancel placebet
                PlaceBetRefund or CancelFT or CancelSingleWager or CancelParlay => new[] {GameActionType.PlaceBet},

                // cancel settlement
                UnsettleFT or UnsettleParlay => new[] {WinBet, LoseBet},

                // cancel cancel
                UncancelFT or UncancelSingleWager or UncancelParlay => new[] {CancelTransaction},

                _ => throw new ArgumentOutOfRangeException(nameof(actionId),
                    $"unrecognized action type id: {actionId}")
            };

        private static bool? GetRoundClosedFlag(ActionType actionId) =>
            actionId switch
            {
                // settle
                SettleFT or SettleParlay => null,

                // cancel placebet
                PlaceBetRefund or CancelFT or CancelSingleWager or CancelParlay => null,

                // cancel settlement
                UnsettleFT or UnsettleParlay => false,

                // cancel cancel
                UncancelFT or UncancelSingleWager or UncancelParlay => false,

                _ => null
            };

        #endregion Private Methods
    }
}