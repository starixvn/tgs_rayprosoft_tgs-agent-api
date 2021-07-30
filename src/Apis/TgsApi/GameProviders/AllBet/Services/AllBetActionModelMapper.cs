using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.AllBet.Common;
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

        BetData VoidRoundRequest(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, TransferRequest request);

        BetData ResettleBetRequest(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, TransferRequest request);
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

            var cancelledReferenceIds = externalIds
                .Where(t => t.GameActionType == CancelTransaction && t.ExternalBetId == betNum)
                .Select(t => t.ExternalReferencedTransactionId).ToArray();

            var reference = externalIds
                .Where(t => t.GameActionType == PlaceBet &&
                    t.ExternalBetId == betNum &&
                    !cancelledReferenceIds.Contains(t.ExternalTransactionId))
                .OrderBy(t => t.RecordedOn)
                .First();

            List<TransactionData> transactions = new();

            if (detail.Deposit > 0)
            {
                transactions.Add(new TransactionData
                {
                    Amount = detail.Deposit,
                    ExternalTransactionId = $"{request.TransactionId}_{betNum}_unlock",
                    ExternalRoundId = reference.ExternalRoundId,
                    ExternalBetId = reference.ExternalBetId,
                    ExternalGameId = reference.ExternalGameId,
                    ExternalGameActionGroupId = request.TransactionId.ToString(),
                    GameActionType = UnlockAmount,
                    RoundClosed = false,
                    TimeStamp = GetTimestamp(detail.RoundEndTime)
                });
            }

            var winOrLossAmount = detail.WinOrLossAmount!.Value;
            var amount = detail.BetAmount + winOrLossAmount;
            transactions.Add(new TransactionData
            {
                AllowLoseBetNegativeAmount = true,
                Amount = amount,
                ExternalTransactionId = $"{request.TransactionId}_{betNum}",
                ExternalTransactionReferenceId = reference.ExternalTransactionId,
                ExternalRoundId = reference.ExternalRoundId,
                ExternalBetId = reference.ExternalBetId,
                ExternalGameId = reference.ExternalGameId,
                ExternalGameActionGroupId = request.TransactionId.ToString(),
                GameActionType = GetGameActionType(detail.BetAmount, winOrLossAmount),
                RoundClosed = forceCloseRound ? true : null,
                TimeStamp = GetTimestamp(detail.RoundEndTime),
                RoundDetail = !forceCloseRound
                    ? null
                    : GetRoundDetail(detail),
                CustomBetValues = new CustomBetValues(detail.ValidAmount!.Value, winOrLossAmount.Negate())
            });

            return GetBetData(playerCode, data, transactions.ToArray(), request.CurrencyCode);
        }

        BetData IAllBetActionModelMapper.VoidRoundRequest(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, TransferRequest request)
        {
            var detail = request.Details[0];
            var betNum = detail.BetNumber.ToString();
            var gameActions = externalIds.Where(t => t.ExternalBetId == betNum).ToArray();

            var cancelledReferenceIds = gameActions
                .Where(t => t.GameActionType == CancelTransaction)
                .Select(t => t.ExternalReferencedTransactionId).ToArray();

            var transactions = gameActions
                .Where(t => !cancelledReferenceIds.Contains(t.ExternalTransactionId))
                .OrderByDescending(t => t.RecordedOn)
                .Select((t, idx) => new TransactionData
                {
                    Amount = t.Amount,
                    ExternalTransactionId = $"{t.ExternalTransactionId}_cancel",
                    ExternalTransactionReferenceId = t.ExternalTransactionId,
                    ExternalRoundId = t.ExternalRoundId,
                    ExternalBetId = t.ExternalBetId,
                    ExternalGameId = t.ExternalGameId,
                    ExternalGameActionGroupId = request.TransactionId.ToString(),
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.UtcNow,
                    RoundClosed = gameActions.Length > 1 && idx == 0 ? false : null
                }).ToArray();

            return GetBetData(playerCode, data, transactions, request.CurrencyCode);
        }

        BetData IAllBetActionModelMapper.ResettleBetRequest(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, TransferRequest request)
        {
            var detail = request.Details[0];
            var betNum = detail.BetNumber.ToString();
            var gameActions = externalIds.Where(t => t.ExternalBetId == betNum).ToArray();

            var cancelledReferenceIds = gameActions
                .Where(t => t.GameActionType == CancelTransaction)
                .Select(t => t.ExternalReferencedTransactionId).ToArray();

            var settles = gameActions.Where(t => t.GameActionType.IsSettleBet() &&
                !cancelledReferenceIds.Contains(t.ExternalTransactionId)).ToArray();

            var cancelSettles = settles.Select(t => new TransactionData
            {
                Amount = t.Amount,
                ExternalTransactionId = $"{t.ExternalTransactionId}_cancel",
                ExternalTransactionReferenceId = t.ExternalTransactionId,
                ExternalRoundId = t.ExternalRoundId,
                ExternalBetId = t.ExternalBetId,
                ExternalGameId = t.ExternalGameId,
                ExternalGameActionGroupId = request.TransactionId.ToString(),
                GameActionType = CancelTransaction,
                TimeStamp = DateTimeOffset.UtcNow,
                RoundClosed = false
            }).ToArray();

            var reference = gameActions.First(t => t.GameActionType == PlaceBet &&
                !cancelledReferenceIds.Contains(t.ExternalTransactionId));

            var winOrLossAmount = detail.WinOrLossAmount!.Value;
            var amount = detail.BetAmount + winOrLossAmount;
            var resettle = new TransactionData
            {
                AllowLoseBetNegativeAmount = true,
                Amount = amount,
                ExternalTransactionId = $"{request.TransactionId}_{betNum}",
                ExternalTransactionReferenceId = reference.ExternalTransactionId,
                ExternalRoundId = reference.ExternalRoundId,
                ExternalBetId = reference.ExternalBetId,
                ExternalGameId = reference.ExternalGameId,
                ExternalGameActionGroupId = request.TransactionId.ToString(),
                GameActionType = GetGameActionType(detail.BetAmount, winOrLossAmount),
                RoundClosed = true,
                TimeStamp = DateTimeOffset.UtcNow,
                RoundDetail = GetRoundDetail(detail),
                CustomBetValues = new CustomBetValues(detail.ValidAmount!.Value, winOrLossAmount.Negate())
            };

            var transactions = cancelSettles.Concat(new[]
            {
                resettle
            }).ToArray();

            return GetBetData(playerCode, data, transactions, request.CurrencyCode);
        }

        BalanceResponse IAllBetActionModelMapper.Response(BetResult result)
        {
            var allTransactionsDuplicated = result.Transactions.All(t => t.IsDuplicate);

            if (allTransactionsDuplicated)
            {
                return new BalanceResponse
                {
                    ErrorCode = (int)GameApiError.InvalidGameActionOperation,
                    ErrorMessage = "Transactions are duplicated"
                };
            }

            return GetBalanceResponse(result.Balance);
        }

        private static GameActionType GetGameActionType(decimal betAmount, decimal winOrLossAmount)
        {
            if (betAmount == winOrLossAmount)
            {
                return TieBet;
            }

            return betAmount + winOrLossAmount > 0 ? WinBet : LoseBet;
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
            string currencyCode = null)
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
                WithCustomBetValues = true
            };

        private static DateTimeOffset GetTimestamp(string timeStamp)
        {
            var offset = TimeSpan.FromHours(8);
            var dateTime = DateTime.ParseExact(timeStamp, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.AllowWhiteSpaces);
            return new DateTimeOffset(dateTime, offset);
        }

        private string GetRoundDetail(TransactionDetail detail)
            => _json.SerializeToString(
                new GameHistory(detail.GameType, detail.GameResult, detail.GameResultExtended),
                new JsonSerializerOptions {WriteIndented = false});
    }
}