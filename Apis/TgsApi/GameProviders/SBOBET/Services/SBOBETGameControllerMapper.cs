using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public interface ISBOBETGameControllerMapper
    {
        BetData Request(GameProviderConfigurationData data, PlaceBetRequest request);

        BetData Request(GameProviderConfigurationData data, string externalRoundId, SettleBetRequest request, GameActionExternalIds[] settleGameActionExternalIds);

        BetData Request(GameProviderConfigurationData data, CancelBetRequest request);

        BetData Request(GameProviderConfigurationData data, CancelBetRequest request, GameActionExternalIds[] validGameActionExternalIds);

        BetData Request(GameProviderConfigurationData data, RollbackRequest request, GameActionExternalIds[] validGameActionExternalIds);

        GetBalanceResponse Response(string playerCode, GetBalanceResult r);

        PlaceBetResponse Response(PlaceBetRequest request, BetResult r);

        SettleBetResponse Response(SettleBetRequest request, BetResult r);

        CancelBetResponse Response(CancelBetRequest request, BetResult r);

        RollbackResponse Response(RollbackRequest request, BetResult r);

        GetBetStatusResponse Response(GetBetStatusRequest request, GameActionData ga);
    }

    public sealed class SBOBETGameControllerMapper : ISBOBETGameControllerMapper
    {
        private const string GameProviderCode = Consts.GameProvider.SBOBET;
        private const bool HasCurrencyFromGameProvider = false;

        BetData ISBOBETGameControllerMapper.Request(GameProviderConfigurationData data, PlaceBetRequest request)
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
                    GameActionType = PlaceBet,
                    TimeStamp = request.BetTime
                }
            };

            return GetBetData(data, request.PlayerCode, transactions);
        }

        BetData ISBOBETGameControllerMapper.Request(GameProviderConfigurationData data, string externalRoundId, SettleBetRequest request, GameActionExternalIds[] settleGameActionExternalIds)
        {
            var settleCount = settleGameActionExternalIds.Length;
            var externalTrasactionId = request.ExternalTransactionId;

            var transactions = new TransactionData[]
            {
                new()
                {
                    Amount = request.WinLoss,
                    ExternalTransactionId = settleCount == 0 ? $"{externalTrasactionId}_settle" : $"{externalTrasactionId}_settle_{settleCount}",
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

        BetData ISBOBETGameControllerMapper.Request(GameProviderConfigurationData data, CancelBetRequest request)
        {
            var externalTrasactionId = request.ExternalTransactionId;
            var externalRoundId = request.ExternalRoundId;

            var transactions = new TransactionData[]
            {
                new()
                {
                    ExternalTransactionId = $"{externalTrasactionId}_cancel",
                    ExternalTransactionReferenceId = externalTrasactionId,
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalTrasactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalGameId = request.ExternalGameId,
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.UtcNow
                }
            };

            return GetBetData(data, request.PlayerCode, transactions);
        }

        BetData ISBOBETGameControllerMapper.Request(GameProviderConfigurationData data, CancelBetRequest request, GameActionExternalIds[] validGameActionExternalIds)
        {
            var transactions = validGameActionExternalIds.OrderByDescending(ga => ga.RecordedOn)
                .Select((ga, idx) => new TransactionData
                {
                    ExternalTransactionId = $"{ga.ExternalTransactionId}_cancel",
                    ExternalTransactionReferenceId = ga.ExternalTransactionId,
                    ExternalRoundId = ga.ExternalRoundId,
                    ExternalBetId = ga.ExternalBetId,
                    ExternalGameActionGroupId = ga.ExternalGameActionGroupId,
                    ExternalGameId = request.ExternalGameId,
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.UtcNow,
                    RoundClosed = idx + 1 == validGameActionExternalIds.Length
                }).ToArray();

            return GetBetData(data, request.PlayerCode, transactions);
        }

        BetData ISBOBETGameControllerMapper.Request(GameProviderConfigurationData data, RollbackRequest request, GameActionExternalIds[] validGameActionExternalIds)
        {
            var lastSettle = validGameActionExternalIds.Where(ga => ga.GameActionType != PlaceBet).Last();

            var transactions = new TransactionData[]
            {
                new()
                {
                    ExternalTransactionId = $"{lastSettle.ExternalTransactionId}_cancel",
                    ExternalTransactionReferenceId = lastSettle.ExternalTransactionId,
                    ExternalRoundId = lastSettle.ExternalRoundId,
                    ExternalBetId = lastSettle.ExternalBetId,
                    ExternalGameActionGroupId = lastSettle.ExternalGameActionGroupId,
                    ExternalGameId = request.ExternalGameId,
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.UtcNow,
                    RoundClosed = false
                }
            };

            return GetBetData(data, request.PlayerCode, transactions);
        }

        GetBalanceResponse ISBOBETGameControllerMapper.Response(string playerCode, GetBalanceResult r) =>
            Response<GetBalanceResponse>(playerCode, r.Balance);

        PlaceBetResponse ISBOBETGameControllerMapper.Response(PlaceBetRequest request, BetResult r) => new()
        {
            ErrorCode = (int)GameApiError.Success,
            ErrorMessage = GameApiError.Success.Description(),
            PlayerCode = request.PlayerCode,
            Balance = r.Balance,
            BetAmount = request.Amount
        };

        SettleBetResponse ISBOBETGameControllerMapper.Response(SettleBetRequest request, BetResult r) =>
            Response<SettleBetResponse>(request.PlayerCode, r.Balance);

        CancelBetResponse ISBOBETGameControllerMapper.Response(CancelBetRequest request, BetResult r) =>
            Response<CancelBetResponse>(request.PlayerCode, r.Balance);

        RollbackResponse ISBOBETGameControllerMapper.Response(RollbackRequest request, BetResult r) =>
            Response<RollbackResponse>(request.PlayerCode, r.Balance);

        GetBetStatusResponse ISBOBETGameControllerMapper.Response(GetBetStatusRequest request, GameActionData ga)
        {
            var roundData = ga.Round;
            var status = GetBetStatus(roundData);

            var externalTransactionId = request.ExternalTransactionId;
            var externalRoundId = request.ExternalRoundId;

            var (stake, winloss) = GetStakeAndWinLoss(status, ga, externalRoundId, externalTransactionId);

            return new GetBetStatusResponse
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMessage = GameApiError.Success.Description(),
                ExternalTransactionId = externalTransactionId,
                ExternalRoundId = externalRoundId,
                Stake = stake,
                WinLoss = winloss,
                Status = status.Description()
            };
        }

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
            ResultType.Tie => TieBet,
            ResultType.Loss => LoseBet,
            _ => WinBet
        };

        private static BetStatus GetBetStatus(RoundRecord roundData) => roundData.Status == RoundStatus.Closed
                    ? roundData.BetAmount == 0 && roundData.WinLoss == 0 ? BetStatus.Void : BetStatus.Settled
                    : BetStatus.Running;

        private static (decimal stake, decimal winloss) GetStakeAndWinLoss(BetStatus status, GameActionData ga, string externalRoundId, string externalTransactionId)
        {
            if (status == BetStatus.Void)
            {
                return (0, 0);
            }

            var cancelledIds = ga.GameActions
                .Where(ga => ga.Type == CancelTransaction && ga.ReferencedGameActionId.HasValue)
                .Select(ga => new { ga.Id, ReferenceId = ga.ReferencedGameActionId } )
                .ToHashSet();

            var validActions = ga.GameActions
                .Where(ga => cancelledIds.All(c => ga.Id != c.Id && ga.Id != c.ReferenceId))
                .ToHashSet();

            var stake = GetStake(validActions, externalRoundId, externalTransactionId);
            var winloss = GetWinLoss(validActions);

            return (stake, winloss);
        }

        private static decimal GetStake(HashSet<GameActionRecord> validActions, string externalRoundId, string externalTransactionId)
        {
            if (externalTransactionId.IgnoreCaseEquals(externalRoundId))
            {
                return validActions
                    .Where(ga => ga.Type == PlaceBet)
                    .Sum(ga => Math.Abs(ga.Amount));
            }

            return validActions
                .Where(ga => ga.ExternalTransactionId == externalTransactionId)
                .Select(ga => Math.Abs(ga.Amount)).FirstOrDefault();
        }

        private static decimal GetWinLoss(HashSet<GameActionRecord> validActions) => 
            validActions.Where(ga => ga.Type != PlaceBet).Sum(ga => ga.Amount);

        private static T Response<T>(string playerCode, decimal balance) where T : BaseResponse, new() => new T
        {
            ErrorCode = (int)GameApiError.Success,
            ErrorMessage = GameApiError.Success.Description(),
            PlayerCode = playerCode,
            Balance = balance
        };

        #endregion Private Methods
    }
}