using System;
using System.Globalization;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
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
    }

    public class Card365ActionModelMapper : ICard365ActionModelMapper
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.Card365;
        private static readonly string _TimestampFormat = "MM/dd/yyyy HH:mm:ss.fff";
        private static readonly TimeSpan _Offset = TimeSpan.FromHours(8);

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