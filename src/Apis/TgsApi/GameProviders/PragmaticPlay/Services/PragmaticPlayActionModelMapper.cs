using System;
using System.Linq;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.PragmaticPlay.Enums;
using TGS.Shared.Game.Models.PragmaticPlay.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public interface IPragmaticPlayActionModelMapper
    {
        PlayerValidationData Request(string playerCode, PlatformType platformType);

        BetData Request(string playerCode, GameProviderConfigurationData data,
            PlaceBetRequest request);

        BetData Request(string playerCode, GameProviderConfigurationData data, ResultRequest request,
            string externalTransactionReferenceId);

        BetData Request(string playerCode, GameProviderConfigurationData data, EndRoundRequest request,
            string externalTransactionReferenceId);

        BetData Request(string playerCode, GameProviderConfigurationData data, RefundRequest request);

        AuthorizePlayerResponse Response(string playerCode, string token, PlayerValidationResult result);

        GetBalanceResponse Response(GetBalanceResult result);

        PlaceBetResponse BetResponse(BetResult result);

        ResultResponse ResultResponse(BetResult result);

        EndRoundResponse EndRoundResponse(decimal balance);

        RefundResponse RefundResponse(BetResult result);
    }

    public sealed class PragmaticPlayActionModelMapper : IPragmaticPlayActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.PragmaticPlay;

        PlayerValidationData IPragmaticPlayActionModelMapper.Request(string playerCode, PlatformType platformType) =>
            new()
            {
                GameProviderCode = GameProviderCode,
                PlayerCode = playerCode,
                PlatformType = platformType
            };

        BetData IPragmaticPlayActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            PlaceBetRequest request)
        {
            var transaction = new TransactionData
            {
                ExternalBetId = request.Reference,
                ExternalGameId = request.GameId,
                ExternalRoundId = request.RoundId,
                ExternalTransactionId = request.Reference,
                ExternalGameActionGroupId = request.RoundId,
                Amount = request.Amount,
                TimeStamp = GetTimestamp(request.Timestamp)
            };

            return GetBetData(playerCode, data, new[] {transaction});
        }

        BetData IPragmaticPlayActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            ResultRequest request, string externalTransactionReferenceId)
        {
            var transaction = new TransactionData
            {
                ExternalBetId = externalTransactionReferenceId,
                ExternalGameId = request.GameId,
                ExternalRoundId = request.RoundId,
                ExternalTransactionId = request.Reference,
                ExternalTransactionReferenceId = externalTransactionReferenceId,
                ExternalGameActionGroupId = request.RoundId,
                GameActionType = GameActionType.WinBet,
                Amount = request.Amount,
                RoundClosed = true,
                TimeStamp = GetTimestamp(request.Timestamp)
            };

            return GetBetData(playerCode, data, new[] {transaction});
        }

        BetData IPragmaticPlayActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            EndRoundRequest request, string externalTransactionReferenceId)
        {
            var transaction = new TransactionData
            {
                ExternalBetId = externalTransactionReferenceId,
                ExternalGameId = request.GameId,
                ExternalRoundId = request.RoundId,
                ExternalTransactionId = $"{externalTransactionReferenceId}_lose",
                ExternalTransactionReferenceId = externalTransactionReferenceId,
                ExternalGameActionGroupId = request.RoundId,
                GameActionType = GameActionType.LoseBet,
                Amount = 0,
                RoundClosed = true,
                TimeStamp = DateTimeOffset.UtcNow
            };

            return GetBetData(playerCode, data, new[] {transaction});
        }

        BetData IPragmaticPlayActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            RefundRequest request)
        {
            var externalTransactionReferenceId = request.Reference;

            var transaction = new TransactionData
            {
                ExternalBetId = externalTransactionReferenceId,
                ExternalGameId = request.GameId,
                ExternalRoundId = request.RoundId,
                ExternalTransactionId = $"{externalTransactionReferenceId}_cancel",
                ExternalTransactionReferenceId = externalTransactionReferenceId,
                ExternalGameActionGroupId = request.RoundId,
                GameActionType = GameActionType.CancelTransaction,
                RoundClosed = true,
                TimeStamp = GetTimestamp(request.Timestamp)
            };

            return GetBetData(playerCode, data, new[] {transaction});
        }

        AuthorizePlayerResponse IPragmaticPlayActionModelMapper.Response(string playerCode, string token,
            PlayerValidationResult result) =>
            new()
            {
                PlayerCode = playerCode,
                Currency = result.GameProviderCurrencyCode,
                Balance = result.Balance,
                Bonus = 0,
                Token = token
            };

        GetBalanceResponse IPragmaticPlayActionModelMapper.Response(GetBalanceResult result) =>
            new()
            {
                Currency = result.GameProviderCurrencyCode,
                Balance = result.Balance,
                Bonus = 0
            };

        PlaceBetResponse IPragmaticPlayActionModelMapper.BetResponse(BetResult result)
        {
            var transaction = result.Transactions.FirstOrDefault();

            return new()
            {
                Error = GameApiError.Success,
                Description = GetTransactionResponseDescription(transaction?.IsDuplicate),
                TransactionId = transaction?.InternalTransactionId?.ToString(),
                Currency = result.GameProviderCurrencyCode,
                Balance = result.Balance,
                Bonus = 0,
                UsedPromo = 0
            };
        }

        ResultResponse IPragmaticPlayActionModelMapper.ResultResponse(BetResult result)
        {
            var transaction = result.Transactions.FirstOrDefault();

            return new()
            {
                Error = GameApiError.Success,
                Description = GetTransactionResponseDescription(transaction?.IsDuplicate),
                TransactionId = transaction?.InternalTransactionId?.ToString(),
                Currency = result.GameProviderCurrencyCode,
                Balance = result.Balance,
                Bonus = 0
            };
        }

        EndRoundResponse IPragmaticPlayActionModelMapper.EndRoundResponse(decimal balance) =>
            new()
            {
                Balance = balance,
                Bonus = 0
            };

        RefundResponse IPragmaticPlayActionModelMapper.RefundResponse(BetResult result)
        {
            var transaction = result.Transactions.FirstOrDefault();

            return new()
            {
                Error = GameApiError.Success,
                Description = GetTransactionResponseDescription(transaction?.IsDuplicate),
                TransactionId = transaction?.InternalTransactionId?.ToString()
            };
        }

        private static BetData GetBetData(string playerCode, GameProviderConfigurationData data,
            TransactionData[] transactions) =>
            new()
            {
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                GameProviderId = data.GameProviderId,
                GameProviderCode = GameProviderCode,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                AutoCloseRound = data.AutoCloseRound,
                ForceGetPreviousActions = true,
                HasCurrencyFromGameProvider = false,
                Transactions = transactions
            };

        private static DateTimeOffset GetTimestamp(long offset)
        {
            try
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(offset);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"unexpected time format: {ex.Message}");
            }
        }

        private static string GetTransactionResponseDescription(bool? isDuplicate) =>
            isDuplicate == true ? "Bet is duplicate" : GameApiError.Success.ToString();
    }
}