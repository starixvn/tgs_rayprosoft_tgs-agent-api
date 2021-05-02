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
        BaseActionResponse Response(GetBalanceResult result);
        BaseActionResponse Response(BetResult result);
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

        BaseActionResponse IJDBActionModelMapper.Response(GetBalanceResult result) =>
            new BaseActionResponse
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
                Amount = Math.Abs(request.Bet),
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
                Amount = request.Win,
                ExternalTransactionId = $"{externalTransactionId}_settle",
                ExternalTransactionReferenceId = externalTransactionId,
                ExternalGameActionGroupId = externalTransactionId,
                ExternalBetId = externalTransactionId,
                ExternalRoundId = externalTransactionId,
                ExternalGameId = request.GameSequenceNo.ToString(),
                GameActionType = request.Win > 0 ? WinBet : LoseBet,
                TimeStamp = DateTimeOffset.Parse(request.LastModifyTime),
                RoundClosed = true
            });

            var platformType = GetPlatformType(request.ClientType);
            return GetBetData(request.UserId, request.Currency, data, transactions.ToArray(), platformType);
        }
        
        BetData IJDBActionModelMapper.Request(GameProviderConfigurationData data, CancelTransactionRequest request)
        {
            var clientRoundId = request.TransferId.ToString();
            
            var transactions = new TransactionData[]
            {
                new()
                {
                    ExternalTransactionId = $"{request.TransferId}_settle_cancel",
                    ExternalTransactionReferenceId = $"{request.TransferId}_settle",
                    ExternalRoundId = clientRoundId,
                    ExternalBetId = clientRoundId,
                    ExternalGameActionGroupId = clientRoundId,
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.FromUnixTimeMilliseconds(request.SystemTime),
                    RoundClosed = false
                },
                new()
                {
                    ExternalTransactionId = $"{request.TransferId}_cancel",
                    ExternalTransactionReferenceId = request.TransferId.ToString(),
                    ExternalRoundId = clientRoundId,
                    ExternalBetId = clientRoundId,
                    ExternalGameActionGroupId = clientRoundId,
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
                Status = SuccessCode,
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
        
        private static string SuccessCode => ((int)GameApiError.Success).ToString().PadLeft(4, '0');

        private static PlatformType GetPlatformType(string platform)
        {
            return "WEB".Equals(platform, StringComparison.OrdinalIgnoreCase)
                ? PlatformType.Desktop
                : PlatformType.Mobile;
        }

        #endregion
    }
}