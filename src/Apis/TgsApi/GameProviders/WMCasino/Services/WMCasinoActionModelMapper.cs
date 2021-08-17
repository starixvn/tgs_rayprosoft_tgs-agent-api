using System;
using System.Globalization;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoActionModelMapper
    {
        BetData LockAmountRequest(string playerCode, GameProviderConfigurationData data, WalletTransferRequest request);

        BetData CancelLockAmountRequest(string playerCode, GameProviderConfigurationData data,
            CancelWalletTransferRequest request);

        GetBalanceResponse Response(string playerCode, GetBalanceResult r);

        WalletTransferResponse Response(WalletTransferRequest request, decimal balance);

        CancelWalletTransferResponse Response(CancelWalletTransferRequest request, decimal balance);
    }

    public class WMCasinoActionModelMapper : IWMCasinoActionModelMapper
    {
        private const string _TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        private const string _GameProviderCode = Consts.GameProvider.WMCasino;
        private static readonly TimeSpan _Offset = TimeSpan.FromHours(8);

        public BetData LockAmountRequest(string playerCode, GameProviderConfigurationData data,
            WalletTransferRequest request)
        {
            var transaction = new TransactionData
            {
                Amount = Math.Abs(request.Amount),
                ExternalGameId = request.ExternalGameId,
                ExternalRoundId = request.ExternalRoundId,
                ExternalTransactionId = $"{request.DealId}_lock",
                ExternalBetId = request.DealId,
                ExternalGameActionGroupId = $"{playerCode}_{request.ExternalRoundId}",
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
            var transaction = new TransactionData
            {
                Amount = Math.Abs(request.Amount),
                ExternalGameId = request.ExternalGameId,
                ExternalRoundId = request.ExternalRoundId,
                ExternalTransactionId = $"{request.DealId}_lock_cancel",
                ExternalTransactionReferenceId = $"{request.DealId}_lock",
                ExternalBetId = request.DealId,
                ExternalGameActionGroupId = $"{playerCode}_{request.ExternalRoundId}",
                GameActionType = GameActionType.CancelTransaction,
                TimeStamp = ParseDateTimeOffset(request.Timestamp),
                RoundClosed = false
            };

            return GetBetData(playerCode, data, new[]
            {
                transaction
            });
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

        private static string GetTimestamp()
            => DateTimeOffset.UtcNow.ToOffset(_Offset).ToString(_TimestampFormat);

        private static DateTimeOffset ParseDateTimeOffset(string timestamp)
        {
            var dateTime = DateTime.ParseExact(timestamp, _TimestampFormat, null,
                DateTimeStyles.AllowWhiteSpaces);

            return new DateTimeOffset(dateTime, _Offset);
        }

        private static BetData GetBetData(string playerCode, GameProviderConfigurationData data,
            TransactionData[] transactions)
            => new BetData
            {
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                AutoCloseRound = data.AutoCloseRound,
                PlatformType = data.PlatformType,
                Transactions = transactions,
                HasCurrencyFromGameProvider = false,
                GameProviderCode = _GameProviderCode,
                AllowPlaceBetWithDifferentPlatformToken = true,
                PlayerCode = playerCode,
                WithCustomBetValues = true
            };
    }
}