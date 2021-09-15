using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Card365.Enums;
using TGS.Shared.Game.Models.Card365.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public interface ICard365ActionModelMapper
    {
        BetData LockAmountRequest(string playerCode, GameProviderConfigurationData data, BetRequest request);

        BalanceResponse Response(GetBalanceResult r);

        BalanceResponse Response(BetResult r);

        BetData CancelLockAmountRequest(string playerCode, GameProviderConfigurationData data,
            GameActionExternalIds[] externalIds, CancelBetRequest request);

        BetData PlaceAndSettleBetRequest(string playerCode, GameProviderConfigurationData data,
            SettleBetRequest request);
    }

    public class Card365ActionModelMapper : ICard365ActionModelMapper
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.Card365;
        private static readonly string _TimestampFormat = "MM/dd/yyyy HH:mm:ss.fff";
        private static readonly TimeSpan _Offset = TimeSpan.FromHours(8);

        private readonly JsonSerializerOptions _jsonSerializerOptions =
            new() { WriteIndented = false };


        public BetData LockAmountRequest(string playerCode, GameProviderConfigurationData data, BetRequest request)
        {
            var externalRoundId = request.ExternalRoundId;

            var transaction = new TransactionData
            {
                Amount = Math.Abs(request.BetAmount),
                ExternalGameId = request.ExternalGameId.ToString(),
                ExternalRoundId = externalRoundId,
                ExternalTransactionId = $"{request.ExternalTransactionId}_lock",
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
            GameActionExternalIds[] externalIds, CancelBetRequest request)
        {
            var transactions = externalIds.Select(ga => new TransactionData
            {
                Amount = ga.Amount,
                ExternalGameId = ga.ExternalGameId,
                ExternalRoundId = ga.ExternalRoundId,
                ExternalTransactionId = $"{ga.ExternalTransactionId}_cancel",
                ExternalTransactionReferenceId = ga.ExternalTransactionId,
                ExternalGameActionGroupId = ga.ExternalGameActionGroupId,
                ExternalBetId = ga.ExternalRoundId,
                GameActionType = GameActionType.CancelTransaction,
                TimeStamp = ParseDateTimeOffset(request.Timestamp),
                RoundClosed = true
            }).ToArray();

            return GetBetData(playerCode, data, transactions);
        }


        public BetData PlaceAndSettleBetRequest(string playerCode, GameProviderConfigurationData data,
            SettleBetRequest request)
        {
            var externalRoundId = request.ExternalRoundId;
            var externalGameId = request.ExternalGameId.ToString();
            var externalTransactionId = request.ExternalTransactionId;

            var validBet = request.ValidBet;
            var playerWinLoss = request.WinLoss;

            var settleAmount = request.RealBetMoney + playerWinLoss;

            var transactions = new[]
            {
                new TransactionData
                {
                    Amount = request.BetAmount,
                    ExternalGameId = externalGameId,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = $"{externalTransactionId}_unlock",
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalBetId = externalRoundId,
                    GameActionType = GameActionType.UnlockAmount,
                    TimeStamp = ParseDateTimeOffset(request.BetTimestamp)
                },
                new TransactionData
                {
                    Amount = request.RealBetMoney,
                    ExternalGameId = externalGameId,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = externalTransactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalBetId = externalRoundId,
                    GameActionType = GameActionType.PlaceBet,
                    TimeStamp = ParseDateTimeOffset(request.BetTimestamp)
                },
                new TransactionData
                {
                    Amount = settleAmount,
                    ExternalGameId = externalGameId,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = $"{externalTransactionId}_settle",
                    ExternalTransactionReferenceId = externalTransactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalBetId = externalRoundId,
                    GameActionType = settleAmount > 0 ? GameActionType.WinBet : GameActionType.LoseBet,
                    TimeStamp = ParseDateTimeOffset(request.SettleTimestamp),
                    Commission = request.Commission,
                    RoundClosed = true,
                    AllowLoseBetNegativeAmount = true,
                    CustomBetValues = new CustomBetValues(validBet, playerWinLoss.Negate()),
                    RoundDetail = JsonSerializer.Serialize(new
                    {
                        request.OutMoney,
                        request.Chair,
                        request.Banker,
                        request.PlayId,
                        request.Tiles
                    }, _jsonSerializerOptions)
                }
            };

            return GetBetData(playerCode, data, transactions);
        }

        public BalanceResponse Response(GetBalanceResult r)
            => new BalanceResponse
            {
                ErrorCode = ((int)GameApiError.Success).ToString(),
                Balance = r.Balance
            };

        public BalanceResponse Response(BetResult r)
            => new BalanceResponse
            {
                ErrorCode = ((int)GameApiError.Success).ToString(),
                Balance = r.Balance
            };

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

        private static DateTimeOffset ParseDateTimeOffset(string timestamp)
        {
            var dateTime = DateTime.ParseExact(timestamp, _TimestampFormat, null,
                DateTimeStyles.AllowWhiteSpaces);

            return new DateTimeOffset(dateTime, _Offset);
        }
    }
}