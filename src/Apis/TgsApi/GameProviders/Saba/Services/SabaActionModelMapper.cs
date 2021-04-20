using System;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.Saba.Enums;
using TGS.Shared.Game.Models.Saba.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Services
{
    public interface ISabaActionModelMapper
    {
        BetData Request(string playerCode, GameProviderConfigurationData data, PlaceBetRequest request);

        GetBalanceResponse Response(string playerCode, GetBalanceResult result);

        PlaceBetResponse Response(string playerCode, PlaceBetRequest.Data body, BetResult r);
    }

    public sealed class SabaActionModelMapper : ISabaActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Saba;
        private static readonly string ExternalGameId = "1";

        private static readonly TimeSpan TimeSpan = TimeSpan.FromHours(-4);

        BetData ISabaActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            PlaceBetRequest request)
        {
            var body = request.Body;

            var transaction = new TransactionData
            {
                ExternalGameId = ExternalGameId,
                ExternalBetId = string.Empty,
                ExternalRoundId = body.TransactionId.ToString(),
                ExternalTransactionId = body.TransactionId.ToString(),
                ExternalGameActionGroupId = body.TransactionId.ToString(),
                Amount = body.ActualAmount,
                TimeStamp = GetTimestamp(body.BetTime),
                GameActionType = GameActionType.LockAmount
            };

            return GetBetData(playerCode, body.CurrencyCode, data, new[] {transaction});
        }

        GetBalanceResponse ISabaActionModelMapper.Response(string playerCode, GetBalanceResult result) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                PlayerCode = playerCode,
                Balance = result.Balance,
                Timestamp = DateTimeOffset.Now.ToOffset(TimeSpan).ToIso8601String(true)
            };

        PlaceBetResponse ISabaActionModelMapper.Response(string playerCode, PlaceBetRequest.Data body, BetResult r) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                ExternalRoundId = body.TransactionId,
                TransactionId = body.TransactionId
            };

        private static BetData GetBetData(string playerCode, int? currency, GameProviderConfigurationData data,
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
                HasCurrencyFromGameProvider = currency.HasValue,
                CurrencyCode = currency?.ToString(),
                Transactions = transactions
            };

        private static DateTimeOffset GetTimestamp(string timestamp) =>
            DateTimeOffset.Parse(timestamp);
    }
}