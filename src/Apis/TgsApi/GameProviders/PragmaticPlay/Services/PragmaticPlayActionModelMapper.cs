using System;
using System.Linq;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.PragmaticPlay.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public interface IPragmaticPlayActionModelMapper
    {
        PlayerValidationData Request(string playerCode, PlatformType platformType);

        BetData Request(string playerCode, GameProviderConfigurationData data,
            PlaceBetRequest request);

        AuthorizePlayerResponse Response(string playerCode, string token, PlayerValidationResult result);
        GetBalanceResponse Response(GetBalanceResult result);
        PlaceBetResponse Response(BetResult result);
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

        PlaceBetResponse IPragmaticPlayActionModelMapper.Response(BetResult result) =>
            new()
            {
                TransactionId = result.Transactions.FirstOrDefault()?.InternalTransactionId.ToString(),
                Currency = result.GameProviderCurrencyCode,
                Balance = result.Balance,
                Bonus = 0,
                UsedPromo = 0
            };

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
    }
}