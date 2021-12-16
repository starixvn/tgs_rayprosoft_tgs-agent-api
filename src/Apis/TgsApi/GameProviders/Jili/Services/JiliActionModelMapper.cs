using System;
using System.Linq;
using System.Numerics;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    internal class JiliActionModelMapper : IJiliActionModelMapper
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.Jili;
        private readonly IJiliPlayerService _playerService;

        public JiliActionModelMapper(
            IJiliPlayerService playerService)
        {
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public JiliAuthorizePlayerResponse AuthorizePlayerResponse(string playerCode, PlayerValidationResult result)
            => new JiliAuthorizePlayerResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                Balance = result.Balance,
                JiliPlayerId = _playerService.ToJiliPlayerId(result.BrandCode, playerCode),
                CurrencyCode = result.GameProviderCurrencyCode
            };

        public (BetData BetData, string ExternalRoundId) BetAndSettleRequest(
            string playerCode,
            GameProviderConfigurationData data,
            JiliPlaceBetAndSettleRequest request)
        {
            var externalGameId = request.ExternalGameId.ToString();
            var externalRoundId = request.ExternalRoundId.ToString();

            var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(request.WagersTime);

            var settleActionType = request.WinloseAmount > 0 ?
                    GameActionType.WinBet :
                    GameActionType.LoseBet;

            var transactions = new TransactionData[]
            {
                new TransactionData
                {
                    ExternalTransactionId = externalRoundId,
                    ExternalGameId = externalGameId,
                    ExternalRoundId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalBetId = externalRoundId,
                    GameActionType = GameActionType.PlaceBet,
                    Amount = request.BetAmount,
                    TimeStamp = timestamp
                },
                new TransactionData
                {
                    ExternalTransactionId = $"{externalRoundId}_settle",
                    ExternalTransactionReferenceId = externalRoundId,
                    ExternalGameId = externalGameId,
                    ExternalRoundId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalBetId = externalRoundId,
                    GameActionType = settleActionType,
                    Amount = request.WinloseAmount,
                    TimeStamp = timestamp,
                    RoundClosed = true
                }
            };

            var betData = new BetData
            {
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                GameProviderCode = _GameProviderCode,
                AutoCloseRound = data.AutoCloseRound,
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                HasCurrencyFromGameProvider = false,
                Transactions = transactions
            };

            return (betData, externalRoundId);
        }

        public BetData CancelFreeBetRequest(BetData freeBetData)
        {
            var freeBetTransaction = freeBetData.Transactions[0];

            var cancelTransactions = new TransactionData[]
            {
                new TransactionData
                {
                    ExternalTransactionId = $"{freeBetTransaction.ExternalTransactionId}_cancel",
                    ExternalTransactionReferenceId = freeBetTransaction.ExternalTransactionId,
                    ExternalGameId = freeBetTransaction.ExternalGameId,
                    ExternalRoundId = freeBetTransaction.ExternalRoundId,
                    ExternalGameActionGroupId = freeBetTransaction.ExternalGameActionGroupId,
                    ExternalBetId = freeBetTransaction.ExternalBetId,
                    GameActionType = GameActionType.CancelTransaction,
                    TimeStamp = DateTimeOffset.UtcNow,
                    RoundClosed = false
                }
            };

            return new BetData
            {
                GameProviderId = freeBetData.GameProviderId,
                GameProviderConfigurationId = freeBetData.GameProviderConfigurationId,
                GameProviderCode = freeBetData.GameProviderCode,
                AutoCloseRound = freeBetData.AutoCloseRound,
                PlayerCode = freeBetData.PlayerCode,
                PlatformType = freeBetData.PlatformType,
                HasCurrencyFromGameProvider = false,
                Transactions = cancelTransactions
            };
        }

        public BetData CancelPlaceAndSettleBetRequest(BetData placeAndSettleBetData)
        {
            var transactions = placeAndSettleBetData.Transactions
                .OrderByDescending(t => t.GameActionType)
                .ThenByDescending(t => t.TimeStamp).ToArray();

            var cancelTransactions = transactions
                .Select((t, idx)
                    =>
                    new TransactionData
                    {
                        ExternalTransactionId = $"{t.ExternalTransactionId}_cancel",
                        ExternalTransactionReferenceId = t.ExternalTransactionId,
                        ExternalGameId = t.ExternalGameId,
                        ExternalRoundId = t.ExternalRoundId,
                        ExternalGameActionGroupId = t.ExternalGameActionGroupId,
                        ExternalBetId = t.ExternalBetId,
                        GameActionType = GameActionType.CancelTransaction,
                        TimeStamp = DateTimeOffset.UtcNow,
                        RoundClosed = idx == transactions.Length - 1
                    }).ToArray();

            return new BetData
            {
                GameProviderId = placeAndSettleBetData.GameProviderId,
                GameProviderConfigurationId = placeAndSettleBetData.GameProviderConfigurationId,
                GameProviderCode = placeAndSettleBetData.GameProviderCode,
                AutoCloseRound = placeAndSettleBetData.AutoCloseRound,
                PlayerCode = placeAndSettleBetData.PlayerCode,
                PlatformType = placeAndSettleBetData.PlatformType,
                HasCurrencyFromGameProvider = false,
                Transactions = cancelTransactions
            };
        }

        public JiliPlaceBetAndSettleResponse FailBetAndSettleResponse(
            int errorCode,
            string errorMessage,
            string jiliPlayerId,
            BetResult result)
            => new JiliPlaceBetAndSettleResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                JiliPlayerId = jiliPlayerId,
                CurrencyCode = result.GameProviderCurrencyCode,
                Balance = result.Balance
            };

        public JiliCancelTransactionResponse FailCancelTransactionResponse(
            int errorCode,
            string errorMessage,
            string jiliPlayerId,
            GetBalanceResult balanceResult)
            => new JiliCancelTransactionResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                JiliPlayerId = jiliPlayerId,
                CurrencyCode = balanceResult.GameProviderCurrencyCode,
                Balance = balanceResult.Balance
            };

        public (BetData BetData, string ExternalRoundId) FreeBetRequest(string playerCode, GameProviderConfigurationData data, JiliPlaceBetAndSettleRequest request)
        {
            var externalGameId = request.ExternalGameId.ToString();
            var externalRoundId = request.ExternalRoundId.ToString();
            var referencedRoundId = request.ReferencedExternalRoundId!.ToString();

            var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(request.WagersTime);

            var transactions = new TransactionData[]
            {
                new TransactionData
                {
                    ExternalTransactionId = externalRoundId,
                    ExternalGameId = externalGameId,
                    ExternalRoundId = externalRoundId,
                    ExternalGameActionGroupId = referencedRoundId,
                    ExternalBetId = externalRoundId,
                    GameActionType = GameActionType.FreeBet,
                    Amount = request.WinloseAmount,
                    TimeStamp = timestamp,
                    RoundClosed = true,
                }
            };

            var betData = new BetData
            {
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                GameProviderCode = _GameProviderCode,
                AutoCloseRound = data.AutoCloseRound,
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                HasCurrencyFromGameProvider = false,
                FreeBetAsSingleRound = true,
                Transactions = transactions
            };

            return (betData, externalRoundId);
        }

        public JiliPlaceBetAndSettleResponse SuccessBetAndSettleResponse(string jiliPlayerId, BetResult result)
        {
            var roundId = result.Transactions.Select(t => t.RoundId).FirstOrDefault();
            var transactionId = new BigInteger(roundId.ToByteArray(), true, false);

            return new JiliPlaceBetAndSettleResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                Balance = result.Balance,
                JiliPlayerId = jiliPlayerId,
                CurrencyCode = result.GameProviderCurrencyCode,
                TransactionId = transactionId
            };
        }

        public JiliCancelTransactionResponse SuccessCancelTransactionResponse(
            string jiliPlayerId,
            GetBalanceResult balanceResult,
            Guid cancelledGameActionId)
        {
            var transactionId = new BigInteger(cancelledGameActionId.ToByteArray(), true, false);

            return new JiliCancelTransactionResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                JiliPlayerId = jiliPlayerId,
                CurrencyCode = balanceResult.GameProviderCurrencyCode,
                Balance = balanceResult.Balance,
                TransactionId = transactionId
            };
        }

        public PlayerValidationData ValidateRequest(string playerCode, PlatformType platformType)
            => new PlayerValidationData
            {
                PlayerCode = playerCode,
                PlatformType = platformType,
                GameProviderCode = Consts.GameProvider.Jili
            };
    }
}
