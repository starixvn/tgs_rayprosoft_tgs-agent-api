using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
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

        BetData Request(string playerCode, GameProviderConfigurationData data, JackpotWinRequest request);

        AuthorizePlayerResponse Response(string playerCode, string token, PlayerValidationResult result);

        GetBalanceResponse Response(GetBalanceResult result);

        PlaceBetResponse BetResponse(BetResult result);

        ResultResponse ResultResponse(BetResult result);

        EndRoundResponse EndRoundResponse(decimal balance);

        RefundResponse RefundResponse(BetResult result);

        JackpotWinResponse JackpotWinResponse(BetResult result);

        BonusWinResponse BonusWinResponse(BonusWinRequest request, GetBalanceResult r);
    }

    public sealed class PragmaticPlayActionModelMapper : IPragmaticPlayActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.PragmaticPlay;
        private readonly IJsonSerializationProvider _json;

        public PragmaticPlayActionModelMapper(IJsonSerializationProvider json)
        {
            _json = json;
        }

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
                Description = AddCustomDescription(AddJackpotData()),
                RoundDetail = AddCustomDescription(AddBonusData()),
                TimeStamp = GetTimestamp(request.Timestamp)
            };

            return GetBetData(playerCode, data, new[] {transaction});

            Dictionary<string, object> AddJackpotData()
            {
                if (request.JackpotId.IsNullOrEmpty())
                {
                    return null;
                }

                return new()
                {
                    {nameof(request.JackpotId), request.JackpotId},
                    {nameof(request.JackpotContribution), request.JackpotContribution},
                    {nameof(request.JackpotDetails), request.JackpotDetails}
                };
            }

            Dictionary<string, object> AddBonusData()
            {
                if (request.BonusCode.IsNullOrEmpty())
                {
                    return null;
                }

                return new() {{nameof(request.BonusCode), request.BonusCode}};
            }
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

        BetData IPragmaticPlayActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            JackpotWinRequest request)
        {
            var extraData = new Dictionary<string, object> {{nameof(request.JackpotId), request.JackpotId}};

            var transaction = new TransactionData
            {
                ExternalBetId = request.Reference,
                ExternalGameId = request.GameId,
                ExternalRoundId = $"{request.RoundId}_jackpot",
                ExternalTransactionId = request.Reference,
                ExternalGameActionGroupId = request.RoundId,
                GameActionType = GameActionType.WinJackpot,
                Amount = request.Amount,
                RoundClosed = true,
                RoundDetail = AddCustomDescription(extraData),
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

        JackpotWinResponse IPragmaticPlayActionModelMapper.JackpotWinResponse(BetResult result)
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

        BonusWinResponse IPragmaticPlayActionModelMapper.BonusWinResponse(BonusWinRequest request,
            GetBalanceResult result) =>
            new()
            {
                TransactionId = request.Reference,
                Currency = result.GameProviderCurrencyCode,
                Balance = result.Balance,
                Bonus = 0
            };

        #region Private Method

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

        private string AddCustomDescription(Dictionary<string, object> items) =>
            items == null || items.Count == 0 ? null : _json.SerializeToString(items, new() {WriteIndented = false});

        #endregion
    }
}