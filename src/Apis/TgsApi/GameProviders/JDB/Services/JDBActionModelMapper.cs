using System;
using System.Collections.Generic;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Habanero.Enums;
using TGS.Shared.Game.Models.JDB.Incoming;
using TGS.Shared.Game.Models.JDB.Outgoing;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public interface IJDBActionModelMapper
    {
        PlayerValidationData Request(string playerCode, PlatformType platformType);
        BetData Request(GameProviderConfigurationData data, PlaceAndSettleBetRequest request);
        BetData Request(GameProviderConfigurationData data, CancelTransactionRequest request);
        BaseResponse Response(GetBalanceResult result);
        BaseResponse Response(BetResult result);
    }

    public sealed class JDBActionModelMapper : IJDBActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.JDB;

        PlayerValidationData IJDBActionModelMapper.Request(string playerCode, PlatformType platformType) =>
            new()
            {
                GameProviderCode = GameProviderCode,
                PlayerCode = playerCode,
                PlatformType = platformType
            };

        BaseResponse IJDBActionModelMapper.Response(GetBalanceResult result) =>
            new BaseResponse
            {
                Status = "0000",
                Balance = result.Balance
            };
        
        BetData IJDBActionModelMapper.Request(GameProviderConfigurationData data, PlaceAndSettleBetRequest request)
        {
            var transactions = new List<TransactionData>(2);
            var externalTransactionId = request.TransferId.ToString();
            
            //place bet
            transactions.Add(new TransactionData
            {
                Amount = request.Bet,
                ExternalTransactionId = externalTransactionId,
                ExternalGameActionGroupId = externalTransactionId,
                ExternalBetId = externalTransactionId,
                ExternalRoundId = externalTransactionId,
                ExternalGameId = request.MachineType.ToString(),
                GameActionType = PlaceBet,
                TimeStamp = DateTimeOffset.Parse(request.GameDate),
                RoundClosed = false
            });
            
            //settle bet
            transactions.Add(new TransactionData
            {
                Amount = request.Bet,
                ExternalTransactionId = $"{externalTransactionId}_settle",
                ExternalGameActionGroupId = externalTransactionId,
                ExternalBetId = externalTransactionId,
                ExternalRoundId = externalTransactionId,
                ExternalGameId = request.GameSeqNo.ToString(),
                GameActionType = request.Win > 0 ? WinBet : LoseBet,
                TimeStamp = DateTimeOffset.Parse(request.LastModifyTime),
                RoundClosed = true
            });
            

            return GetBetData(request.UserId, request.Currency, data, transactions.ToArray());
        }
        
        BetData IJDBActionModelMapper.Request(GameProviderConfigurationData data, CancelTransactionRequest request)
        {
            var currencyCode = string.Empty;
            var clientRoundId = request.TransferId.ToString();
            
            var transactions = new TransactionData[]
            {
                new()
                {
                    ExternalTransactionId = $"{request.TransferId}_cancel",
                    ExternalTransactionReferenceId = $"{request.TransferId}_place",
                    ExternalRoundId = clientRoundId,
                    ExternalBetId = clientRoundId,
                    ExternalGameActionGroupId = clientRoundId,
                    ExternalGameId = string.Empty, //TODO
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.FromUnixTimeMilliseconds(request.SystemTime)
                }
            };

            return GetBetData(request.UserId, currencyCode, data, transactions);
        }
        
        BaseResponse IJDBActionModelMapper.Response(BetResult result) =>
            new BaseResponse
            {
                Status = SuccessCode,
                Balance = result.Balance
            };

        private string SuccessCode => ((int)GameApiError.Success).ToString().PadLeft(4, '0');

        #region Private Methods

        private static BetData GetBetData(string playerCode, string currencyCode, GameProviderConfigurationData data,
            TransactionData[] transactions) => new()
        {
            GameProviderId = data.GameProviderId,
            GameProviderConfigurationId = data.GameProviderConfigurationId,
            GameProviderCode = GameProviderCode,
            AutoCloseRound = data.AutoCloseRound,
            PlayerCode = playerCode,
            PlatformType = data.PlatformType,
            HasCurrencyFromGameProvider = true,
            CurrencyCode = currencyCode,
            Transactions = transactions
        };
        
        #endregion
    }
}