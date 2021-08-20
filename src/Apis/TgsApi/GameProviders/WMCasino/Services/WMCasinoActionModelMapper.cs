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
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Incoming;
using BetResult = TGS.Shared.Game.Models.WMCasino.Incoming.BetResult;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoActionModelMapper
    {
        BetData LockAmountRequest(string playerCode, GameProviderConfigurationData data, WalletTransferRequest request);

        BetData CancelLockAmountRequest(string playerCode, GameProviderConfigurationData data,
            CancelWalletTransferRequest request);

        BetData PlaceAndSettleBetRequest(
            GameProviderConfigurationData data, string playerCode, BetResult[] results,
            GameActionExternalIds[] externalIds, bool isManual);

        GetBalanceResponse Response(string playerCode, GetBalanceResult r);

        WalletTransferResponse Response(WalletTransferRequest request, decimal balance);

        CancelWalletTransferResponse Response(CancelWalletTransferRequest request, decimal balance);

        ErrorResponse Response(GameApiError error, string message);
    }

    public class WMCasinoActionModelMapper : IWMCasinoActionModelMapper
    {
        private const string _TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        private const string _GameProviderCode = Consts.GameProvider.WMCasino;
        private static readonly TimeSpan _Offset = TimeSpan.FromHours(8);

        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false
        };

        private readonly IJsonSerializationProvider _json;

        private readonly IWMCasinoUtility _utility;

        public WMCasinoActionModelMapper(
            IWMCasinoUtility utility,
            IJsonSerializationProvider json)
        {
            _utility = utility;
            _json = json;
        }

        public BetData LockAmountRequest(string playerCode, GameProviderConfigurationData data,
            WalletTransferRequest request)
        {
            var externalGameId = request.ExternalGameId;
            var externalRoundId = _utility.GetExternalRoundIdByGameNumber(request.GameNo, externalGameId);

            var transaction = new TransactionData
            {
                Amount = Math.Abs(request.Amount),
                ExternalGameId = externalGameId,
                ExternalRoundId = externalRoundId,
                ExternalTransactionId = $"{request.DealId}_lock",
                ExternalGameActionGroupId = externalRoundId,
                ExternalBetId = externalRoundId,
                GameActionType = GameActionType.LockAmount,
                TimeStamp = ParseDateTimeOffset(request.Timestamp)
            };

            return GetBetData(playerCode, data, new[]
            {
                transaction
            });
        }

        public BetData CancelLockAmountRequest(string playerCode, GameProviderConfigurationData data,
            CancelWalletTransferRequest request)
        {
            var externalGameId = request.ExternalGameId;
            var externalRoundId = _utility.GetExternalRoundIdByGameNumber(request.GameNo, externalGameId);

            var transaction = new TransactionData
            {
                Amount = Math.Abs(request.Amount),
                ExternalGameId = externalGameId,
                ExternalRoundId = externalRoundId,
                ExternalTransactionId = $"{request.DealId}_lock_cancel",
                ExternalTransactionReferenceId = $"{request.DealId}_lock",
                ExternalGameActionGroupId = externalRoundId,
                ExternalBetId = externalRoundId,
                GameActionType = GameActionType.CancelTransaction,
                TimeStamp = ParseDateTimeOffset(request.Timestamp),
                RoundClosed = false
            };

            return GetBetData(playerCode, data, new[]
            {
                transaction
            });
        }

        public BetData PlaceAndSettleBetRequest(
            GameProviderConfigurationData data, string playerCode, BetResult[] results,
            GameActionExternalIds[] externalIds, bool isManual)
        {
            var bets = new List<TransactionData>();
            var settles = new List<TransactionData>();


            // lock amount is stored as negative in database

            var remainingLockAmount = CalculateRemainingLockAmount(externalIds);
            var lockedAmount = remainingLockAmount;

            foreach (var r in results)
            {
                var betId = r.BetId;

                if (externalIds.Any(t => t.ExternalTransactionId == betId))
                {
                    continue;
                }

                var betTypeCode = GetBetTypeCode(isManual, r);
                var betAmount = r.BetAmount;
                var winAmount = betAmount + r.WinLoss;

                var betTime = ParseDateTimeOffset(r.BetTime);
                var externalGameId = r.ExternalGameId;
                var externalRoundId = _utility.GetExternalRoundIdByBetResult(r.RoundId, r.SubRoundId);

                lockedAmount += betAmount;

                bets.AddRange(new[]
                {
                    new TransactionData
                    {
                        Amount = betAmount,
                        ExternalTransactionId = $"{betId}_unlock",
                        ExternalRoundId = externalRoundId,
                        ExternalGameActionGroupId = externalRoundId,
                        ExternalBetId = externalRoundId,
                        ExternalGameId = externalGameId,
                        GameActionType = GameActionType.UnlockAmount,
                        TimeStamp = betTime
                    },
                    new TransactionData
                    {
                        Amount = betAmount,
                        ExternalTransactionId = betId,
                        ExternalRoundId = externalRoundId,
                        ExternalGameActionGroupId = externalRoundId,
                        ExternalBetId = betId,
                        ExternalGameId = externalGameId,
                        GameActionType = GameActionType.PlaceBet,
                        BetType = betTypeCode,
                        TimeStamp = betTime,
                        ExtraData = _json.SerializeToString(
                            new { BetDeailIds = r.BetWalletIds.SplitByComma(StringSplitOptions.RemoveEmptyEntries) },
                            _serializerOptions)
                    }
                });

                settles.Add(new TransactionData
                {
                    Amount = winAmount,
                    ExternalTransactionId = $"{betId}_settle",
                    ExternalRoundId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalBetId = betId,
                    ExternalTransactionReferenceId = betId,
                    ExternalGameId = externalGameId,
                    GameActionType = winAmount == 0 ? GameActionType.LoseBet :
                        winAmount > betAmount       ? GameActionType.WinBet : GameActionType.TieBet,
                    TimeStamp = ParseDateTimeOffset(r.SettleTime),
                    RoundDetail = r.GameResult,
                    RoundClosed = lockedAmount == 0,
                    ExtraData = _json.SerializeToString(
                        new { BetDeailIds = r.BetWalletIds.SplitByComma(StringSplitOptions.RemoveEmptyEntries) },
                        _serializerOptions),
                    CustomBetValues = new CustomBetValues(r.ValidBet, r.WinLoss.Negate())
                });
            }

            var transactions = bets.Concat(settles).ToArray();
            return GetBetData(playerCode, data, transactions, true);
        }

        public GetBalanceResponse Response(string playerCode, GetBalanceResult r)
            => new GetBalanceResponse
            {
                ErrorCode = (int)GameApiError.Success,
                Data = new GetBalanceResponse.Result
                {
                    PlayerCode = playerCode,
                    Balance = r.Balance,
                    Timestamp = GetTimestamp()
                }
            };

        public WalletTransferResponse Response(WalletTransferRequest request, decimal balance)
            => new WalletTransferResponse
            {
                ErrorCode = (int)GameApiError.Success,
                Data = new WalletTransferResponse.Result
                {
                    Amount = request.Amount,
                    Balance = balance,
                    DealId = request.DealId,
                    Timestamp = GetTimestamp()
                }
            };

        public CancelWalletTransferResponse Response(CancelWalletTransferRequest request, decimal balance)
            => new CancelWalletTransferResponse
            {
                ErrorCode = (int)GameApiError.Success,
                Data = new CancelWalletTransferResponse.Result
                {
                    Amount = request.Amount,
                    Balance = balance,
                    DealId = request.DealId,
                    Timestamp = GetTimestamp()
                }
            };

        public ErrorResponse Response(GameApiError error, string message)
            => new ErrorResponse
            {
                ErrorCode = (int)error,
                ErrorMessage = message
            };

        private static string GetTimestamp()
            => DateTimeOffset.UtcNow.ToOffset(_Offset).ToString(_TimestampFormat);

        private static DateTimeOffset ParseDateTimeOffset(string timestamp)
        {
            var dateTime = DateTime.ParseExact(timestamp, _TimestampFormat, null,
                DateTimeStyles.AllowWhiteSpaces);

            return new DateTimeOffset(dateTime, _Offset);
        }

        private static BetData GetBetData(string playerCode, GameProviderConfigurationData data,
            TransactionData[] transactions, bool allowPlaceBetWithDifferentPlatformToken = false)
            => new BetData
            {
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                AutoCloseRound = data.AutoCloseRound,
                PlatformType = data.PlatformType,
                Transactions = transactions,
                HasCurrencyFromGameProvider = false,
                GameProviderCode = _GameProviderCode,
                AllowPlaceBetWithDifferentPlatformToken = allowPlaceBetWithDifferentPlatformToken,
                PlayerCode = playerCode,
                WithCustomBetValues = true
            };

        private static string GetBetTypeCode(bool isManual, BetResult r)
            => isManual ? r.BetTypeCode : r.BetType;

        private static decimal CalculateRemainingLockAmount(GameActionExternalIds[] externalIds)
        {
            var cancellationReferenceIds = externalIds
                .Where(t => t.GameActionType == GameActionType.CancelTransaction)
                .Select(t => t.ExternalReferencedTransactionId)
                .ToArray();

            return externalIds
                .Where(t =>
                    t.GameActionType.IsOneOf(GameActionType.LockAmount, GameActionType.UnlockAmount) &&
                    !cancellationReferenceIds.Contains(t.ExternalTransactionId))
                .Sum(t => t.Amount);
        }
    }
}