using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
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

        BetData SettleBetRequest(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, TransferRequest request, bool forceCloseRound);
    }

    public class AllBetActionModelMapper : IAllBetActionModelMapper
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.AllBet;
        private readonly IJsonSerializationProvider _json;

        public AllBetActionModelMapper(IJsonSerializationProvider json)
        {
            _json = json;
        }

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

        BetData IAllBetActionModelMapper.SettleBetRequest(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, TransferRequest request, bool forceCloseRound)
        {
            var detail = request.Details[0];
            var betNum = detail.BetNumber.ToString();
            var reference = externalIds.Where(t => t.GameActionType == PlaceBet && t.ExternalBetId == betNum)
                .OrderBy(t => t.RecordedOn).First();

            List<TransactionData> transactions = new();

            if (detail.Deposit > 0)
            {
            }


            var amount = detail.BetAmount + detail.ValidAmount!.Value;
            transactions.Add(new TransactionData
            {
                Amount = amount,
                ExternalTransactionId = $"{request.TransactionId}_{betNum}",
                ExternalTransactionReferenceId = reference.ExternalTransactionId,
                ExternalRoundId = reference.ExternalRoundId,
                ExternalBetId = reference.ExternalBetId,
                ExternalGameId = reference.ExternalGameId,
                ExternalGameActionGroupId = request.TransactionId.ToString(),
                GameActionType = GetGameActionType(detail.ValidAmount!.Value),
                RoundClosed = forceCloseRound ? true : null,
                TimeStamp = GetTimestamp(detail.RoundEndTime),
                RoundDetail = !forceCloseRound
                    ? null
                    : _json.SerializeToString(new
                    {
                        Result = detail.GameResult,
                        Extended = detail.GameResultExtended
                    }, new JsonSerializerOptions {WriteIndented = false})
            });

            return GetBetData(playerCode, data, transactions.ToArray(), request.CurrencyCode);
        }

        private static GameActionType GetGameActionType(decimal validAmount)
            => validAmount switch
            {
                > 0 => WinBet,
                < 0 => LoseBet,
                0   => TieBet
            };

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