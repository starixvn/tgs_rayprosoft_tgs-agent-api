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

        BetResponse Response(TransactionData[] reqTrans, BetResult r);

        BatchBetData Request(Guid gameProviderId, Dictionary<string, Guid> playerConfigurationIds, 
            IDictionary<string, IEnumerable<GameActionExternalIds>> externalIdList, BetRequest request, bool isSettle);

        IEnumerable<TransactionResponse> Response(BatchTransactionData[] reqTrans, BatchBetResult r);
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

        BetData IIMESportsActionModelMapper.Request(GameProviderConfigurationData data, BetRequest request, string playerCode,
            bool allowPlaceBetWithDifferentPlatformToken)
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
                ExtraData = t.ExternalTransactionId,
                TimeStamp = t.Timestamp
            }).ToArray();

            return GetBetData(data, playerCode, transactions, allowPlaceBetWithDifferentPlatformToken);
        }

        BetResponse IIMESportsActionModelMapper.Response(TransactionData[] reqTrans, BetResult r)
            => new()
            {
                Results = (from res in r.Transactions
                          join req in reqTrans
                              on res.ExternalTransactionId equals req.ExternalTransactionId
                          select new TransactionResponse
                {
                    InternalTransactionId = res.InternalTransactionId.Value,
                    ExternalTransactionId = req.ExtraData,
                    Balance = r.Balance
                }).ToArray()
            };

        BatchBetData IIMESportsActionModelMapper.Request(Guid gameProviderId,
            Dictionary<string, Guid> playerConfigurationIds, 
            IDictionary<string, IEnumerable<GameActionExternalIds>> externalIdList, 
            BetRequest request, bool isSettle)
        {
            return new()
            {
                ExternalBatchId = Guid.NewGuid().ToString(),
                GameProviderId = gameProviderId,
                GameProviderCode = GameProviderCode,
                AutoCloseRound = true,
                AllowCancelCancelTransaction = true,
                Transactions = request.Transactions.Where(t => GetGameActionType(t.ActionType, t.Amount).IsSettleBet() == isSettle).
                                                    Select(t =>
                {
                    playerConfigurationIds.TryGetValue(t.PlayerCode, out var gameProviderConfigurationid);
                    var externalIds = externalIdList[t.ExternalRoundId];
                    var externalId = externalIds.First(e => GetRefGameActionTypes(t.ActionType).Contains(e.GameActionType));
                    
                    return new BatchTransactionData
                    {
                        PlayerCode = t.PlayerCode,
                        GameProviderConfigurationId = gameProviderConfigurationid,
                        Amount = Math.Abs(t.Amount),
                        ExternalRoundId = t.ExternalRoundId,
                        ExternalBetId = t.ExternalRoundId,
                        ExternalGameId = t.ExternalGameId,
                        ExternalTransactionId = t.ExternalTransactionId,
                        ExternalTransactionReferenceId = externalId.ExternalTransactionId,
                        GameActionType = GetGameActionType(t.ActionType, t.Amount),
                        RoundClosed = isSettle ? (bool?)null : false,
                        CurrencyCode = t.Currency,
                        Description = t.ExternalTransactionId,
                        Timestamp = t.Timestamp
                    };
                }).ToArray()
            };
        }

        IEnumerable<TransactionResponse> IIMESportsActionModelMapper.Response(BatchTransactionData[] reqTrans, BatchBetResult r)
        {
            var results = new List<TransactionResponse>();
            
            results.AddRange(
                from result in r.Results
                join balance in r.PlayerBalances
                    on result.PlayerCode equals balance.Key
                join req in reqTrans
                    on result.ExternalTransactionId equals req.ExternalTransactionId
                select new TransactionResponse
                {
                    ExternalTransactionId = req.Description,
                    InternalTransactionId = result.InternalTransactionId.Value,
                    Balance = balance.Value
                });
            
            results.AddRange(
                from error in r.Errors
                join req in reqTrans
                    on error.ExternalTransactionId equals req.ExternalTransactionId
                select new TransactionResponse
                {
                    ExternalTransactionId = req.Description,
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

        private static GameActionType GetGameActionType(ActionType actionId, decimal amount)
        {
            switch (actionId)
            {
                case SettleFT:
                case SettleParlay: // settle
                    return amount > 0 ? WinBet : LoseBet;

                // cancel placebet
                case PlaceBetRefund:
                case CancelFT:
                case CancelSingleWager:
                case CancelParlay: 
                // cancel settlement
                case UnsettleFT:
                case UnsettleParlay:
                // cancel cancel
                case UncancelFT:
                case UncancelSingleWager:
                case UncancelParlay:
                    return CancelTransaction;

                default:
                    throw new ArgumentOutOfRangeException(nameof(actionId), $"unrecognized action type id: {actionId}");
            }
        }
        
        private static GameActionType[] GetRefGameActionTypes(ActionType actionId)
        {
            switch (actionId)
            {
                case SettleFT:
                case SettleParlay:
                case PlaceBetRefund:
                case CancelFT:
                case CancelSingleWager:
                case CancelParlay:
                    return new []{ GameActionType.PlaceBet };

                case UncancelFT:
                case UncancelSingleWager:
                case UncancelParlay:
                    return new []{ CancelTransaction };
                
                case UnsettleFT:
                case UnsettleParlay:
                    return new []{ WinBet, LoseBet };

                default:
                    throw new ArgumentOutOfRangeException(nameof(actionId), $"unrecognized action type id: {actionId}");
            }
        }

        #endregion Private Methods
    }
}