using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.AsiaGaming.Common;
using TGS.Shared.Game.Models.AsiaGaming.Enums;
using TGS.Shared.Game.Models.AsiaGaming.Incoming;
using TransactionType = TGS.Shared.Game.Models.AsiaGaming.Enums.TransactionType;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public interface IAsiaGamingActionModelMapper
    {
        BetData Request(string playerCode, GameProviderConfigurationData data, PostTransferRequest r, TransactionType transactionType, bool lockGame, OppositeBetSetting[] oppositeBetSettings);

        BetData LockAmountRequest(string playerCode, GameProviderConfigurationData data, PostTransferRequest r);

        BetData PlaceBetRequest(string playerCode, GameProviderConfigurationData data, BetResponseRequest r, GameActionExternalIds externalIds, OppositeBetSetting[] oppositeBetSettings);

        TransferResponse Response(GameApiError error, GetBalanceResult result);
        
        TransferResponse Response(BetResult result);
    }

    public sealed class AsiaGamingActionModelMapper : IAsiaGamingActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.AsiaGaming;
        private readonly IJsonSerializationProvider _jsonSerialization;

        public AsiaGamingActionModelMapper(IJsonSerializationProvider jsonSerialization)
        {
            _jsonSerialization = jsonSerialization;
        }
        
        BetData IAsiaGamingActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            PostTransferRequest r, TransactionType transactionType, bool lockGame, OppositeBetSetting[] oppositeBetSettings)
        {
            var isPlaceBet = transactionType == TransactionType.BET;
            var isPlaceBetOrCancel = isPlaceBet || transactionType == TransactionType.REFUND;
            var gameActionType = GetGameActionType(transactionType);
            var currencyCode = r.Record.Currency;
            var roundClosed = isPlaceBetOrCancel ? false : bool.TryParse(r.Record.RoundFinish, out var finish) ? finish : (bool?)null;
            var transactionId = GetTransactionId(transactionType, r.Record.TransactionId);
            var timestamp = isPlaceBetOrCancel ? r.Record.BetTime : r.Record.SettleTime;
            var amount = isPlaceBetOrCancel ? Math.Abs(r.Record.Amount) : r.Record.BetAmount + r.Record.WinAmount;
            
            var transactions = new TransactionData[]
            {
                new()
                {
                    Amount = amount,
                    ExternalTransactionId = transactionId,
                    ExternalTransactionReferenceId = isPlaceBet ? string.Empty : GetTransactionReferenceId(r.Record, lockGame),
                    ExternalRoundId = r.Record.RoundId,
                    ExternalBetId = transactionId,
                    ExternalGameId = GetGameId(r),
                    GameActionType = gameActionType,
                    TimeStamp = GetTimestamp(timestamp),
                    RoundClosed = roundClosed,
                    RoundDetail = GetRoundHistory(roundClosed, r.Record.GameResult),
                }
            };
            
            return GetBetData(playerCode, data, transactions, currencyCode, 
                oppositeBetSettings: oppositeBetSettings);
        }

        BetData IAsiaGamingActionModelMapper.LockAmountRequest(string playerCode, GameProviderConfigurationData data,
            PostTransferRequest r)
        {
            var currencyCode = r.Record.Currency;
            var transactionId = r.Record.TransactionId;
            var timestamp = r.Record.BetTime;
            var amount = Math.Abs(r.Record.Amount);
            
            var transactions = new TransactionData[]
            {
                new()
                {
                    Amount = amount,
                    ExternalTransactionId = transactionId,
                    ExternalRoundId = r.Record.RoundId,
                    ExternalBetId = transactionId,
                    ExternalGameId = GetGameId(r),
                    GameActionType = GameActionType.LockAmount,
                    TimeStamp = GetTimestamp(timestamp),
                    RoundDetail = r.Record.GameResult,
                }
            };
            
            return GetBetData(playerCode, data, transactions, currencyCode);
        }

        BetData IAsiaGamingActionModelMapper.PlaceBetRequest(string playerCode, GameProviderConfigurationData data, 
            BetResponseRequest r, GameActionExternalIds externalIds, OppositeBetSetting[] oppositeBetSettings)
        {
            var transactionId = r.TransactionId;

            var transactions = new List<TransactionData>
            {
                new()
                {
                    Amount = Math.Abs(externalIds.Amount),
                    ExternalTransactionId = $"{transactionId}-{GameActionType.UnlockAmount}",
                    ExternalRoundId = externalIds.ExternalRoundId,
                    ExternalBetId = transactionId,
                    ExternalGameId = externalIds.ExternalGameId,
                    GameActionType = GameActionType.UnlockAmount,
                    TimeStamp = externalIds.RecordedOn
                }
            };
            transactions.AddRange(r.Records.Select(re => new TransactionData
            {
                Amount = re.Amount,
                ExternalTransactionId = $"{transactionId}-{re.BillNo}",
                ExternalRoundId = externalIds.ExternalRoundId,
                ExternalBetId = $"{transactionId}-{re.BillNo}",
                ExternalGameId = externalIds.ExternalGameId,
                GameActionType = GameActionType.PlaceBet,
                TimeStamp = externalIds.RecordedOn
            }));
            
            return GetBetData(playerCode, data, transactions.ToArray(), 
                hasCurrencyFromGameProvider: false, oppositeBetSettings: oppositeBetSettings);
        }

        TransferResponse IAsiaGamingActionModelMapper.Response(GameApiError error, GetBalanceResult result)
        {
            return new()
            {
                ResponseCode = error,
                Balance = result == null ? 0 : result.Balance
            };
        }

        TransferResponse IAsiaGamingActionModelMapper.Response(BetResult result)
        {
            return new()
            {
                ResponseCode = GameApiError.OK,
                Balance = result.Balance
            };
        }

        #region Private Methods

        private static GameActionType GetGameActionType(TransactionType transactionType)
        {
            return transactionType switch
            {
                TransactionType.WIN => GameActionType.WinBet,
                TransactionType.LOSE => GameActionType.LoseBet,
                TransactionType.REFUND => GameActionType.CancelTransaction,
                _ => GameActionType.PlaceBet,
            };
        }
        
        private static string GetTransactionId(TransactionType transactionType, string transactionId)
        {
            return transactionType == TransactionType.BET ? transactionId : $"{transactionId}_{transactionType}";
        }
        
        private static string GetTransactionReferenceId(TransferRecord record, bool lockGame)
        {
            return lockGame ? $"{record.TransactionId}-{record.BillNo}" : record.TransactionId;
        }

        private static string GetGameId(PostTransferRequest r)
        {
            return $"{r.Record.GamePlatformType}_{r.Record.GameRound}_{r.Record.GameType}";
        }
        
        private static DateTimeOffset GetTimestamp(string betTime)
        {
            var offset = TimeSpan.FromHours(-4);
            var dateTime = DateTime.ParseExact(betTime, "MM/dd/yyyy HH:mm:sss", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            return new DateTimeOffset(dateTime, offset);
        }

        private string GetRoundHistory(bool? roundClosed, string roundResult)
        {
            return roundClosed == true ? _jsonSerialization.SerializeToString(new GameHistory
            {
                RoundResult = roundResult
            }) : string.Empty;
        }
        
        private static BetData GetBetData(string playerCode, GameProviderConfigurationData data,
            TransactionData[] transactions, string currencyCode = null, 
            bool hasCurrencyFromGameProvider = true, OppositeBetSetting[] oppositeBetSettings = null) => new()
        {
            GameProviderId = data.GameProviderId,
            GameProviderConfigurationId = data.GameProviderConfigurationId,
            GameProviderCode = GameProviderCode,
            AutoCloseRound = data.AutoCloseRound,
            PlayerCode = playerCode,
            PlatformType = data.PlatformType,
            HasCurrencyFromGameProvider = hasCurrencyFromGameProvider,
            CurrencyCode = currencyCode,
            Transactions = transactions,
            OppositeBetRules = oppositeBetSettings?.Select(o => new OppositeBetRule
            {
                ExternalGameIds = o.Games,
                Patterns = o.Patterns?.Select(p => new OppositeBetRule.Pattern(p.BetType, p.GroupId)).ToArray()
            }).ToArray()
        };

        #endregion
    }
}