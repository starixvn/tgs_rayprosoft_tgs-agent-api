using System;
using System.Globalization;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.AllBet.Enums;
using TGS.Shared.Game.Models.AllBet.Incoming;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public interface IAllBetActionModelMapper
    {
        BalanceResponse Response(GetBalanceResult result);

        BalanceResponse Response(BetResult result);

        BetData BetRequest(string playerCode, GameProviderConfigurationData data, TransferRequest request);

        BetData CancelBetRequest(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, CancelTransferRequest request);
    }

    public class AllBetActionModelMapper : IAllBetActionModelMapper
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.AllBet;

        BalanceResponse IAllBetActionModelMapper.Response(GetBalanceResult result)
            => GetBalanceResponse(result.Balance);

        BalanceResponse IAllBetActionModelMapper.Response(BetResult result)
            => GetBalanceResponse(result.Balance);

        BetData IAllBetActionModelMapper.BetRequest(string playerCode, GameProviderConfigurationData data,
            TransferRequest request)
        {
            var transactions = request.Details.SelectMany(t =>
            {
                var externalTransactionId = $"{request.TransactionId}_{t.BetNumber}";
                var externalRoundId = t.RoundId.ToString();
                var externalGameId = t.GameType.ToString();
                var externalGameActionGroupId = request.TransactionId.ToString();
                var externalBetId = t.BetNumber.ToString();

                var bet = new TransactionData
                {
                    Amount = t.BetAmount,
                    ExternalTransactionId = externalTransactionId,
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalBetId,
                    ExternalGameId = externalGameId,
                    ExternalGameActionGroupId = externalGameActionGroupId,
                    GameActionType = PlaceBet,
                    TimeStamp = GetTimestamp(t.BetTime),
                    BetType = t.BetType.ToString()
                };

                if (t.Deposit == 0)
                {
                    return new[]
                    {
                        bet
                    };
                }

                var tlock = new TransactionData
                {
                    Amount = t.Deposit,
                    ExternalTransactionId = $"{externalTransactionId}_lock",
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalBetId,
                    ExternalGameId = externalGameId,
                    ExternalGameActionGroupId = externalGameActionGroupId,
                    GameActionType = LockAmount,
                    TimeStamp = GetTimestamp(t.BetTime)
                };

                return new[]
                {
                    bet,
                    tlock
                };
            }).ToArray();

            return GetBetData(playerCode, data, transactions, request.CurrencyCode);
        }

        BetData IAllBetActionModelMapper.CancelBetRequest(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, CancelTransferRequest request)
        {
            var transactions = externalIds
                .OrderByDescending(t => t.RecordedOn)
                .Select(t => new TransactionData
                {
                    Amount = t.Amount,
                    ExternalTransactionId = $"{t.ExternalTransactionId}_cancel",
                    ExternalTransactionReferenceId = t.ExternalTransactionId,
                    ExternalRoundId = t.ExternalRoundId,
                    ExternalBetId = t.ExternalBetId,
                    ExternalGameId = t.ExternalGameId,
                    ExternalGameActionGroupId = request.TransactionId.ToString(),
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.UtcNow
                }).ToArray();

            return GetBetData(playerCode, data, transactions);
        }

        private static BalanceResponse GetBalanceResponse(decimal balance)
            => new BalanceResponse
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = GameApiError.Success.ToString(),
                Balance = balance,
                Version = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

        private static BetData GetBetData(
            string playerCode,
            GameProviderConfigurationData data,
            TransactionData[] transactions,
            string currencyCode = null,
            OppositeBetSetting[] oppositeBetSettings = null)
            => new BetData
            {
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                GameProviderCode = _GameProviderCode,
                AutoCloseRound = data.AutoCloseRound,
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                HasCurrencyFromGameProvider = currencyCode.IsNotNullOrEmpty(),
                CurrencyCode = currencyCode,
                Transactions = transactions,
                OppositeBetRules = oppositeBetSettings?.Select(o => new OppositeBetRule
                {
                    ExternalGameIds = o.Games,
                    Patterns = o.Patterns?.Select(p => new OppositeBetRule.Pattern(p.BetType, p.GroupId)).ToArray()
                }).ToArray()
            };

        private static DateTimeOffset GetTimestamp(string timeStamp)
        {
            var offset = TimeSpan.FromHours(8);
            var dateTime = DateTime.ParseExact(timeStamp, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.AllowWhiteSpaces);
            return new DateTimeOffset(dateTime, offset);
        }
    }
}