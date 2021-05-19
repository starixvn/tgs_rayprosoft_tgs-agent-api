using System;
using System.Collections.Generic;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.JDB.Enums;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public interface IJDBActionModelMapper
    {
        BetData Request(GameProviderConfigurationData data, PlaceAndSettleBetRequest request, PlatformType platformType);
        BetData Request(GameProviderConfigurationData data, CancelTransactionRequest request, string externalRoundId);
        BaseActionResponse Response(GetBalanceResult result);
        BaseActionResponse Response(BetResult result);
    }

    public sealed class JDBActionModelMapper : IJDBActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.JDB;

        BaseActionResponse IJDBActionModelMapper.Response(GetBalanceResult result) =>
            new BaseActionResponse
            {
                Status = GameApiStatus.Succeed.Description(),
                Balance = result.Balance
            };
        
        BetData IJDBActionModelMapper.Request(GameProviderConfigurationData data, PlaceAndSettleBetRequest request, PlatformType platformType)
        {
            var transactions = new List<TransactionData>(2);
            var externalTransactionId = request.TransferId.ToString();
            var externalRoundId = request.GameSequenceNo.ToString();

            //place bet
            transactions.Add(new TransactionData
            {
                Amount = Math.Abs(request.Bet),
                ExternalTransactionId = externalTransactionId,
                ExternalGameActionGroupId = externalRoundId,
                ExternalBetId = externalTransactionId,
                ExternalRoundId = externalRoundId,
                ExternalGameId = request.MachineType.ToString(),
                GameActionType = PlaceBet,
                TimeStamp = DateTimeOffset.Parse(request.GameDate),
                RoundClosed = false
            });
            
            //settle bet
            transactions.Add(new TransactionData
            {
                Amount = request.Win,
                ExternalTransactionId = $"{externalTransactionId}_settle",
                ExternalTransactionReferenceId = externalTransactionId,
                ExternalGameActionGroupId = externalRoundId,
                ExternalBetId = externalTransactionId,
                ExternalRoundId = externalRoundId,
                ExternalGameId = request.GameSequenceNo.ToString(),
                GameActionType = request.Win > 0 ? WinBet : LoseBet,
                TimeStamp = DateTimeOffset.Parse(request.LastModifyTime),
                RoundClosed = true
            });
            
            return GetBetData(request.UserId, request.Currency, data, transactions.ToArray(), platformType);
        }
        
        BetData IJDBActionModelMapper.Request(GameProviderConfigurationData data, CancelTransactionRequest request, string externalRoundId)
        {
            var externalTransactionId = request.TransferId.ToString();

            var transactions = new TransactionData[]
            {
                new()
                {
                    ExternalTransactionId = $"{request.TransferId}_settle_cancel",
                    ExternalTransactionReferenceId = $"{request.TransferId}_settle",
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalTransactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.FromUnixTimeMilliseconds(request.SystemTime),
                    RoundClosed = false
                },
                new()
                {
                    ExternalTransactionId = $"{request.TransferId}_cancel",
                    ExternalTransactionReferenceId = request.TransferId.ToString(),
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalTransactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.FromUnixTimeMilliseconds(request.SystemTime),
                    RoundClosed = true
                }
            };

            return GetBetData(request.UserId, null, data, transactions, null, hasCurrencyFromGameProvider: false);
        }
        
        BaseActionResponse IJDBActionModelMapper.Response(BetResult result) =>
            new BaseActionResponse
            {
                Status = GameApiStatus.Succeed.Description(),
                Balance = result.Balance
            };

        #region Private Methods

        private static BetData GetBetData(string playerCode, string currencyCode, GameProviderConfigurationData data,
            TransactionData[] transactions, PlatformType? platform, bool hasCurrencyFromGameProvider = true) => new()
        {
            GameProviderId = data.GameProviderId,
            GameProviderConfigurationId = data.GameProviderConfigurationId,
            GameProviderCode = GameProviderCode,
            AutoCloseRound = data.AutoCloseRound,
            PlayerCode = playerCode,
            PlatformType = platform,
            HasCurrencyFromGameProvider = hasCurrencyFromGameProvider,
            CurrencyCode = currencyCode,
            Transactions = transactions
        };

        #endregion
    }
}