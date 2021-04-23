using System;
using System.Linq;
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

        BetData Request(string playerCode, GameProviderConfigurationData data, PlaceBetParlayRequest request);

        BetData Request(string playerCode, GameProviderConfigurationData data, ConfirmBetRequest request,
            decimal unlockAmount);

        BetData Request(string playerCode, GameProviderConfigurationData data, CancelBetRequest request);

        GetBalanceResponse Response(string playerCode, GetBalanceResult result);

        PlaceBetResponse Response(string playerCode, PlaceBetRequest.Data body, BetResult r);

        PlaceBetParlayResponse Response(string playerCode, PlaceBetParlayRequest.Data body, BetResult r);

        ConfirmBetResponse Response(string playerCode, ConfirmBetRequest.Data body, BetResult r);

        CancelBetResponse Response(string playerCode, CancelBetRequest.Data body, BetResult r);
    }

    public sealed class SabaActionModelMapper : ISabaActionModelMapper
    {
        private static readonly string GameProviderCode = Consts.GameProvider.Saba;
        private static readonly TimeSpan TimeSpan = TimeSpan.FromHours(-4);

        private readonly string _externalGameId = SabaConsts.ExternalGameId;

        BetData ISabaActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            PlaceBetRequest request)
        {
            var body = request.Body;
            var externalRoundId = body.ReferenceId.ToString();

            var transaction = new TransactionData
            {
                ExternalGameId = _externalGameId,
                ExternalBetId = string.Empty,
                ExternalRoundId = externalRoundId,
                ExternalTransactionId = externalRoundId,
                ExternalGameActionGroupId = externalRoundId,
                Amount = body.ActualAmount,
                TimeStamp = GetTimestamp(body.BetTime),
                GameActionType = GameActionType.LockAmount
            };

            return GetBetData(playerCode, body.CurrencyCode, data, new[] {transaction});
        }

        BetData ISabaActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            PlaceBetParlayRequest request)
        {
            var body = request.Body;

            var transactions = body.Transactions.Select(t =>
            {
                var externalRoundId = t.ReferenceId.ToString();

                return new TransactionData
                {
                    ExternalGameId = _externalGameId,
                    ExternalBetId = string.Empty,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    Amount = t.BetAmount,
                    TimeStamp = GetTimestamp(body.BetTime),
                    GameActionType = GameActionType.LockAmount
                };
            }).ToArray();

            return GetBetData(playerCode, body.CurrencyCode, data, transactions);
        }

        BetData ISabaActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            ConfirmBetRequest request, decimal unlockAmount)
        {
            var body = request.Body;
            var confirmBet = body.Transactions[0];

            var externalTransactionId = confirmBet.ExternalTransactionId.ToString();
            var externalRoundId = confirmBet.ExternalRoundId.ToString();

            var transactions = new TransactionData[]
            {
                new()
                {
                    ExternalGameId = _externalGameId,
                    ExternalBetId = string.Empty,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = $"{externalRoundId}_unlock",
                    ExternalTransactionReferenceId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    Amount = unlockAmount,
                    TimeStamp = GetTimestamp(body.UpdateTime),
                    GameActionType = GameActionType.UnlockAmount
                },
                new()
                {
                    ExternalGameId = _externalGameId,
                    ExternalBetId = externalTransactionId,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = externalTransactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    Amount = confirmBet.ActualAmount,
                    TimeStamp = GetTimestamp(body.UpdateTime),
                    GameActionType = GameActionType.PlaceBet
                }
            };

            return GetBetData(playerCode, null, data, transactions);
        }

        BetData ISabaActionModelMapper.Request(string playerCode, GameProviderConfigurationData data,
            CancelBetRequest request)
        {
            var body = request.Body;
            var transactions = body.Transactions.Select(t =>
            {
                var externalRoundId = t.ExternalRoundId.ToString();

                return new TransactionData
                {
                    ExternalGameId = _externalGameId,
                    ExternalBetId = string.Empty,
                    ExternalRoundId = externalRoundId,
                    ExternalTransactionId = $"{externalRoundId}_lock_cancel",
                    ExternalTransactionReferenceId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    TimeStamp = GetTimestamp(body.UpdateTime),
                    GameActionType = GameActionType.CancelTransaction
                };
            }).ToArray();

            return GetBetData(playerCode, null, data, transactions);
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
                ExternalRoundId = body.ReferenceId,
                ReferenceId = body.ReferenceId
            };

        PlaceBetParlayResponse ISabaActionModelMapper.Response(string playerCode, PlaceBetParlayRequest.Data body,
            BetResult r) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                Transactions = body.Transactions.Select(t => new PlaceBetParlayResponse.Transaction
                {
                    ExternalRoundId = t.ReferenceId,
                    ReferenceId = t.ReferenceId
                }).ToArray()
            };

        ConfirmBetResponse ISabaActionModelMapper.
            Response(string playerCode, ConfirmBetRequest.Data body, BetResult r) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                Balance = r.Balance
            };

        CancelBetResponse ISabaActionModelMapper.Response(string playerCode, CancelBetRequest.Data body, BetResult r) =>
            new()
            {
                Status = ((int)GameApiError.Success).ToString(),
                Balance = r.Balance
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