using System;
using System.Linq;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Habanero.Enums;
using TGS.Shared.Game.Models.Habanero.Incoming;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Services
{
    public interface IHabaneroActionModelMapper
    {
        PlayerValidationData Request(string playerCode, PlatformType platformType);

        BetData Request(string playerCode, GameProviderConfigurationData data, FundTransferRequest r);

        AuthorizePlayerResponse Response(string playerCode, PlayerValidationResult r);

        AuthorizePlayerResponse Response(string playerCode, GetBalanceResult r);

        AuthorizePlayerResponse Response(GameApiError error, string message);

        FundTransferResponse Response(FundTransferRequest request, BetResult result);

        FundTransferResponse Response(GameApiError error, string message, FundTransferRequest request,
            GetBalanceResult result);

        QueryResponse Response(bool success);
    }

    public sealed class HabaneroActionModelMapper : IHabaneroActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Habanero;

        PlayerValidationData IHabaneroActionModelMapper.Request(string playerCode, PlatformType platformType) =>
            new()
            {
                GameProviderCode = GameProviderCode,
                PlayerCode = playerCode,
                PlatformType = platformType
            };

        AuthorizePlayerResponse IHabaneroActionModelMapper.Response(string playerCode, PlayerValidationResult r) =>
            new()
            {
                Result = new()
                {
                    Status = new() {HasAuthenticationError = false},
                    PlayerCode = playerCode,
                    PlayerUsername = r.Username,
                    Balance = r.Balance,
                    CurrencyCode = r.GameProviderCurrencyCode
                }
            };

        BetData IHabaneroActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            FundTransferRequest r)
        {
            string currencyCode;
            TransactionData[] transactions;

            if (r.Detail.IsCancel)
            {
                var trx = r.Detail.FundTransfer.CancelTransaction;
                var clientRoundId = r.Detail.ClientRoundId.ToString();

                currencyCode = trx.CurrencyCode;
                transactions = new TransactionData[]
                {
                    new()
                    {
                        Amount = Math.Abs(trx.Amount),
                        ExternalTransactionId = trx.TransactionId,
                        ExternalTransactionReferenceId = trx.TransactionReferenceId,
                        ExternalGameActionGroupId = clientRoundId,
                        ExternalBetId = r.Detail.RoundId,
                        ExternalRoundId = clientRoundId,
                        ExternalGameId = r.Game.ExternalId,
                        GameActionType = GameActionType.CancelTransaction,
                        TimeStamp = DateTimeOffset.Parse(trx.Timestamp),
                        RoundClosed = trx.GameState is GameState.Completing or GameState.Expired
                    }
                };
            }
            else
            {
                var trxs = r.Detail.FundTransfer.Transactions;
                currencyCode = trxs[0].CurrencyCode;

                transactions = trxs.Select(t =>
                {
                    var clientRoundId = r.Detail.ClientRoundId.ToString();

                    return new TransactionData
                    {
                        Amount = Math.Abs(t.Amount),
                        ExternalTransactionId = t.TransactionId,
                        ExternalTransactionReferenceId = t.IsPlaceBet ? null : t.TransactionReferenceId,
                        ExternalGameActionGroupId = clientRoundId,
                        ExternalBetId = r.Detail.RoundId,
                        ExternalRoundId = clientRoundId,
                        ExternalGameId = r.Game.ExternalId,
                        GameActionType =
                            t.IsPlaceBet                              ? PlaceBet :
                            r.Detail.ActionType == ActionType.FreeBet ? FreeBet :
                            t.Amount == 0                             ? LoseBet : WinBet,
                        TimeStamp = DateTimeOffset.Parse(t.Timestamp),
                        RoundClosed = t.GameState is GameState.Completing or GameState.Expired
                    };
                }).ToArray();
            }

            return GetBetData(playerCode, currencyCode, data, transactions);
        }

        AuthorizePlayerResponse IHabaneroActionModelMapper.Response(string playerCode, GetBalanceResult r) =>
            new()
            {
                Result = new()
                {
                    Status = new() {HasAuthenticationError = false},
                    PlayerCode = playerCode,
                    PlayerUsername = r.Username,
                    Balance = r.Balance,
                    CurrencyCode = r.GameProviderCurrencyCode
                }
            };

        AuthorizePlayerResponse IHabaneroActionModelMapper.Response(GameApiError error, string message) =>
            new()
            {
                Result = new()
                {
                    Status = new()
                    {
                        Success = false,
                        Message = $"code: {(int)error}, message: {message}"
                    }
                }
            };

        FundTransferResponse IHabaneroActionModelMapper.Response(FundTransferRequest request, BetResult result)
        {
            var actionType = request.Detail.ActionType;

            return new()
            {
                Result = new()
                {
                    Status = new()
                    {
                        Success = true,
                        DebitSucceeded = actionType == ActionType.PlaceAndSettleBet ? true : null,
                        CreditSucceeded = actionType == ActionType.PlaceAndSettleBet ? true : null,
                        CancelStatus = request.Detail.IsCancel ? CancelStatus.Cancelled : null
                    },
                    Balance = result.Balance,
                    CurrencyCode = result.GameProviderCurrencyCode
                }
            };
        }

        FundTransferResponse IHabaneroActionModelMapper.Response(GameApiError error, string message,
            FundTransferRequest request, GetBalanceResult result)
        {
            var status = error switch
            {
                GameApiError.AuthorizationFailed => new()
                {
                    Success = false,
                    HasAuthenticationError = true
                },
                GameApiError.InsufficientFunds => new()
                {
                    Success = false,
                    HasInsufficientFunds = true
                },
                _ => GetFundTransferStatusWithDefaultErrors(error, message, request)
            };

            return new()
            {
                Result = new()
                {
                    Status = status,
                    Balance = result?.Balance,
                    CurrencyCode = result?.GameProviderCurrencyCode
                }
            };
        }

        QueryResponse IHabaneroActionModelMapper.Response(bool success) =>
            new() {Result = new() {Status = new() {Success = success}}};

        #region Private Methods

        private static BetData GetBetData(string playerCode, string currencyCode, GameProviderConfigurationData data,
            TransactionData[] transactions) => new()
        {
            GameProviderId = data.GameProviderId,
            GameProviderConfigurationId = data.GameProviderConfigurationId,
            GameProviderCode = GameProviderCode,
            AutoCloseRound = data.AutoCloseRound,
            PlayerCode = playerCode,
            PlatformType = data.PlatformType,
            HasCurrencyFromGameProvider = true,
            CurrencyCode = currencyCode,
            Transactions = transactions
        };

        private static FundTransferStatus GetFundTransferStatusWithDefaultErrors(GameApiError error, string message,
            FundTransferRequest request)
        {
            if (request.Detail.IsCancel)
            {
                var isTransactionNotFound = error == GameApiError.TransactionNotExist;

                return new()
                {
                    Success = isTransactionNotFound,
                    CancelStatus = isTransactionNotFound ? CancelStatus.TransactionNotFound : CancelStatus.None,
                    Message = message
                };
            }

            var isBetAndSettle = request.Detail.FundTransfer.IsBetAndSettle;
            var success = error == GameApiError.Success;

            return new()
            {
                Success = success,
                Message = success ? string.Empty : message,
                DebitSucceeded = isBetAndSettle ? false : null,
                CreditSucceeded = isBetAndSettle ? false : null
            };
        }

        #endregion
    }
}