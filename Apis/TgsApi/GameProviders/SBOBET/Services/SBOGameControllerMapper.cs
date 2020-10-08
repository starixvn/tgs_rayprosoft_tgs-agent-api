using System;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public interface ISBOGameControllerMapper
    {
        BetData Request(GameProviderConfigurationData data, PlaceBetRequest request);

        BetData Request(GameProviderConfigurationData data, string externalRoundId, SettleBetRequest request);

        GetBalanceResponse Response(string playerCode, GetBalanceResult r);

        PlaceBetResponse Response(PlaceBetRequest request, BetResult r);

        SettleBetResponse Response(SettleBetRequest request, BetResult r);
    }

    public sealed class SBOGameControllerMapper : ISBOGameControllerMapper
    {
        private const string GameProviderCode = Consts.GameProvider.SBOBET;
        private const bool HasCurrencyFromGameProvider = false;

        BetData ISBOGameControllerMapper.Request(GameProviderConfigurationData data, PlaceBetRequest request)
        {
            var externalTransactionId = request.ExternalTransactionId;
            var externalRoundId = request.ExternalRoundId;

            var transactions = new TransactionData[]
            {
                new()
                {
                    Amount = request.Amount,
                    ExternalTransactionId = externalTransactionId,
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalTransactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalGameId = request.ExternalGameId,
                    GameActionType = GameActionType.PlaceBet,
                    TimeStamp = request.BetTime
                }
            };

            return GetBetData(data, request.PlayerCode, transactions);
        }

        BetData ISBOGameControllerMapper.Request(GameProviderConfigurationData data, string externalRoundId, SettleBetRequest request)
        {
            var externalTrasactionId = request.ExternalTransactionId;

            var transactions = new TransactionData[]
            {
                new()
                {
                    Amount = request.WinLoss,
                    ExternalTransactionId = $"{externalTrasactionId}_settle",
                    ExternalTransactionReferenceId = externalTrasactionId,
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = request.ExternalTransactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalGameId = request.ExternalGameId,
                    GameActionType = GetGameActionType(request.Result),
                    TimeStamp = request.SettleTime
                }
            };

            return GetBetData(data, request.PlayerCode, transactions);
        }

        GetBalanceResponse ISBOGameControllerMapper.Response(string playerCode, GetBalanceResult r) => new()
        {
            ErrorCode = (int)GameApiError.Success,
            ErrorMessage = GameApiError.Success.Description(),
            PlayerCode = playerCode,
            Balance = r.Balance
        };

        PlaceBetResponse ISBOGameControllerMapper.Response(PlaceBetRequest request, BetResult r) => new()
        {
            ErrorCode = (int)GameApiError.Success,
            ErrorMessage = GameApiError.Success.Description(),
            PlayerCode = request.PlayerCode,
            Balance = r.Balance,
            BetAmount = request.Amount
        };

        SettleBetResponse ISBOGameControllerMapper.Response(SettleBetRequest request, BetResult r) => new()
        {
            ErrorCode = (int)GameApiError.Success,
            ErrorMessage = GameApiError.Success.Description(),
            PlayerCode = request.PlayerCode,
            Balance = r.Balance
        };

        #region Private Methods

        private static BetData GetBetData(GameProviderConfigurationData data, string playerCode, TransactionData[] transactions) => new()
        {
            PlayerCode = playerCode,
            PlatformType = data.PlatformType,
            GameProviderId = data.GameProviderId,
            GameProviderConfigurationId = data.GameProviderConfigurationId,
            GameProviderCode = GameProviderCode,
            HasCurrencyFromGameProvider = HasCurrencyFromGameProvider,
            AutoCloseRound = data.AutoCloseRound,
            Transactions = transactions
        };

        private static GameActionType GetGameActionType(ResultType result) => result switch
        {
            ResultType.Win => GameActionType.WinBet,
            ResultType.Tie => GameActionType.TieBet,
            ResultType.Loss => GameActionType.LoseBet
        };

        #endregion Private Methods
    }
}