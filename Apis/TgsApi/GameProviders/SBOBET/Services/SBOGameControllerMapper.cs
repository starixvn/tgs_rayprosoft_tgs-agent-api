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
        BetData Request(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType, PlaceBetRequest request);

        GetBalanceResponse Response(string playerCode, GetBalanceResult r);

        PlaceBetResponse Response(PlaceBetRequest request, BetResult r);
    }

    public sealed class SBOGameControllerMapper : ISBOGameControllerMapper
    {
        private const string GameProviderCode = Consts.GameProvider.SBOBET;
        private const bool HasCurrencyFromGameProvider = false;

        BetData ISBOGameControllerMapper.Request(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType, PlaceBetRequest request)
        {
            return new()
            {
                PlayerCode = request.PlayerCode,
                PlatformType = platformType,
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                GameProviderCode = GameProviderCode,
                HasCurrencyFromGameProvider = HasCurrencyFromGameProvider,
                Transactions = new TransactionData[]
                {
                    new()
                    {
                        Amount = request.Amount,
                        ExternalTransactionId = request.ExternalTransactionId,
                        ExternalRoundId = request.ExternalRoundId,
                        ExternalBetId = request.ExternalTransactionId,
                        ExternalGameActionGroupId = request.ExternalRoundId,
                        ExternalGameId = request.ExternalGameId,
                        GameActionType = GameActionType.PlaceBet,
                        TimeStamp = request.BetTime
                    }
                }
            };
        }

        GetBalanceResponse ISBOGameControllerMapper.Response(string playerCode, GetBalanceResult r)
        {
            return new()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = GameApiError.Success.Description(),
                PlayerCode = playerCode,
                Balance = r.Balance
            };
        }

        PlaceBetResponse ISBOGameControllerMapper.Response(PlaceBetRequest request, BetResult r)
        {
            return new()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = GameApiError.Success.Description(),
                PlayerCode = request.PlayerCode,
                Balance = r.Balance,
                BetAmount = request.Amount
            };
        }
    }
}