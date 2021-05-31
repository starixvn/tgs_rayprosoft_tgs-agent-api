// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Shared.Game.Models.SBOBET.Enums;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public interface ISBOBETActionModelMapper
    {
        BetData Request(GameProviderConfigurationData data, PlaceBetRequest request,
            bool allowPlaceBetWithDifferentPlatformToken = false);

        BetData CancelAndPlaceBetRequest(GameProviderConfigurationData data, PlaceBetRequest request,
            bool allowPlaceBetWithDifferentPlatformToken = false);

        BetData Request(GameProviderConfigurationData data, string externalRoundId, SettleBetRequest request,
            bool hasRollback, bool forceCloseRound = false);

        BetData Request(GameProviderConfigurationData data, CancelBetRequest request);

        BetData Request(GameProviderConfigurationData data, CancelBetRequest request,
            GameActionExternalIds[] validGameActionExternalIds);

        BetData Request(GameProviderConfigurationData data, RollbackRequest request,
            GameActionExternalIds[] gameActionExternalIds);

        BatchBetData Request(Guid gameProviderId, Dictionary<string, Guid> playerConfigurationIds, bool autoCloseRound,
            BatchRequest request);

        GetBalanceResponse Response(string playerCode, GetBalanceResult r);

        PlaceBetResponse Response(PlaceBetRequest request, BetResult r);

        SettleBetResponse Response(SettleBetRequest request, BetResult r);

        CancelBetResponse Response(CancelBetRequest request, BetResult r);

        RollbackResponse Response(RollbackRequest request, BetResult r);

        GetBetStatusResponse Response(GetBetStatusRequest request, GameActionData ga);

        T Response<T>(string playerCode, GameApiError errorCode) where T : BaseResponse, new();

        BatchResponse Response(BatchBetResult r);
    }

    public sealed class SBOBetActionModelMapper : ISBOBETActionModelMapper
    {
        private const string GameProviderCode = Consts.GameProvider.SBOBET;
        private const bool HasCurrencyFromGameProvider = false;

        BetData ISBOBETActionModelMapper.Request(GameProviderConfigurationData data, CancelBetRequest request)
        {
            var externalTransactionReferenceId = request.ExternalTransactionId;
            var externalRoundId = request.ExternalRoundId;

            var transactions = new TransactionData[]
            {
                new()
                {
                    ExternalTransactionId = $"{externalTransactionReferenceId}_cancel",
                    ExternalTransactionReferenceId = externalTransactionReferenceId,
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalTransactionReferenceId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalGameId = request.GameType == 0 ? null : request.ExternalGameId,
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.UtcNow
                }
            };

            return GetBetData(data, request.PlayerCode, transactions);
        }

        BetData ISBOBETActionModelMapper.Request(GameProviderConfigurationData data, CancelBetRequest request,
            GameActionExternalIds[] validGameActionExternalIds)
        {
            var transactions = validGameActionExternalIds
                .OrderByDescending(ga => ga.RecordedOn)
                .Select((ga, idx) => new TransactionData
                {
                    ExternalTransactionId = $"{ga.ExternalTransactionId}_cancel",
                    ExternalTransactionReferenceId = ga.ExternalTransactionId,
                    ExternalRoundId = ga.ExternalRoundId,
                    ExternalBetId = ga.ExternalBetId,
                    ExternalGameActionGroupId = ga.ExternalGameActionGroupId,
                    ExternalGameId = request.GameType == 0 ? null : request.ExternalGameId,
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.UtcNow,
                    RoundClosed = idx + 1 == validGameActionExternalIds.Length
                }).ToArray();

            return GetBetData(data, request.PlayerCode, transactions);
        }

        BetData ISBOBETActionModelMapper.Request(GameProviderConfigurationData data, RollbackRequest request,
            GameActionExternalIds[] gameActionExternalIds)
        {
            var externalIds = gameActionExternalIds.OrderBy(ga => ga.RecordedOn).ToArray();
            var last = externalIds.Last();
            var reference =
                externalIds
                    .FirstOrDefault(ga => ga.ExternalTransactionId == last.ExternalReferencedTransactionId);

            var action = reference != null && reference.GameActionType.IsSettleBet() &&
                last.GameActionType == CancelTransaction
                    ? reference
                    : last;

            var transactions = new TransactionData[]
            {
                new()
                {
                    ExternalTransactionId = $"{action.ExternalTransactionId}_cancel",
                    ExternalTransactionReferenceId = action.ExternalTransactionId,
                    ExternalRoundId = action.ExternalRoundId,
                    ExternalBetId = action.ExternalBetId,
                    ExternalGameActionGroupId = action.ExternalGameActionGroupId,
                    ExternalGameId = request.GameType == 0 ? null : request.ExternalGameId,
                    GameActionType = CancelTransaction,
                    TimeStamp = DateTimeOffset.UtcNow,
                    RoundClosed = false
                }
            };

            return GetBetData(data, request.PlayerCode, transactions);
        }

        GetBalanceResponse ISBOBETActionModelMapper.Response(string playerCode, GetBalanceResult r) =>
            Response<GetBalanceResponse>(playerCode, r.Balance);

        PlaceBetResponse ISBOBETActionModelMapper.Response(PlaceBetRequest request, BetResult r)
        {
            var isDuplicate = r.Transactions.Any(t => t.IsDuplicate);

            var errorCode = isDuplicate ? GameApiError.BetAlreadyExists : GameApiError.Success;
            decimal? balance = isDuplicate ? null : r.Balance;

            return new()
            {
                ErrorCode = (int)errorCode,
                ErrorMessage = errorCode.Description(),
                PlayerCode = request.PlayerCode,
                Balance = balance,
                BetAmount = request.Amount
            };
        }

        SettleBetResponse ISBOBETActionModelMapper.Response(SettleBetRequest request, BetResult r) =>
            r.Transactions.Any(t => t.IsDuplicate)
                ? Response<SettleBetResponse>(request.PlayerCode, r.Balance, GameApiError.BetAlreadySettled)
                : Response<SettleBetResponse>(request.PlayerCode, r.Balance);

        CancelBetResponse ISBOBETActionModelMapper.Response(CancelBetRequest request, BetResult r) =>
            r.Transactions.Any(t => t.IsDuplicate)
                ? Response<CancelBetResponse>(request.PlayerCode, r.Balance, GameApiError.BetAlreadyCanceled)
                : Response<CancelBetResponse>(request.PlayerCode, r.Balance);

        RollbackResponse ISBOBETActionModelMapper.Response(RollbackRequest request, BetResult r) =>
            r.Transactions.Any(t => t.IsDuplicate)
                ? Response<RollbackResponse>(request.PlayerCode, r.Balance, GameApiError.BetAlreadyRollback)
                : Response<RollbackResponse>(request.PlayerCode, r.Balance);

        GetBetStatusResponse ISBOBETActionModelMapper.Response(GetBetStatusRequest request, GameActionData ga)
        {
            var roundData = ga.Round;
            var status = GetBetStatus(roundData);

            var externalTransactionId = request.ExternalTransactionId;
            var externalRoundId = request.ExternalRoundId;

            var (stake, winloss) = GetStakeAndWinLoss(status, ga, externalRoundId, externalTransactionId);

            return new()
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

        T ISBOBETActionModelMapper.Response<T>(string playerCode, GameApiError errorCode) =>
            Response<T>(playerCode, 0, errorCode);

        BetData ISBOBETActionModelMapper.Request(
            GameProviderConfigurationData data,
            string externalRoundId,
            SettleBetRequest request,
            bool hasRollback,
            bool forceCloseRound)
        {
            var externalTransactionReferenceId = externalRoundId;

            var externalTransactionId = !hasRollback
                ? $"{externalTransactionReferenceId}_settle"
                : $"{externalTransactionReferenceId}_resettle";

            var transactions = new TransactionData[]
            {
                new()
                {
                    Amount = request.WinLoss,
                    ExternalTransactionId = externalTransactionId,
                    ExternalTransactionReferenceId = externalTransactionReferenceId,
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalRoundId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalGameId = request.GameType == 0 ? null : request.ExternalGameId,
                    GameActionType = GetGameActionType(request.Result),
                    TimeStamp = request.SettleTime,
                    RoundClosed = forceCloseRound ? true : null
                }
            };

            return GetBetData(data, request.PlayerCode, transactions);
        }

        BetData ISBOBETActionModelMapper.Request(GameProviderConfigurationData data, PlaceBetRequest request,
            bool allowPlaceBetWithDifferentPlatformToken)
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

            return GetBetData(data, request.PlayerCode, transactions, allowPlaceBetWithDifferentPlatformToken);
        }

        BetData ISBOBETActionModelMapper.CancelAndPlaceBetRequest(GameProviderConfigurationData data,
            PlaceBetRequest request, bool allowPlaceBetWithDifferentPlatformToken)
        {
            var externalTransactionId = request.ExternalTransactionId;
            var externalRoundId = request.ExternalRoundId;

            var transactions = new TransactionData[]
            {
                new()
                {
                    Amount = request.Amount,
                    ExternalTransactionId = $"{externalTransactionId}_cancel",
                    ExternalTransactionReferenceId = externalTransactionId,
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalTransactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalGameId = request.ExternalGameId,
                    GameActionType = CancelTransaction,
                    TimeStamp = request.BetTime,
                    RoundClosed = false
                },
                new()
                {
                    Amount = request.Amount,
                    ExternalTransactionId = $"{externalTransactionId}_raise",
                    ExternalRoundId = externalRoundId,
                    ExternalBetId = externalTransactionId,
                    ExternalGameActionGroupId = externalRoundId,
                    ExternalGameId = request.ExternalGameId,
                    GameActionType = PlaceBet,
                    TimeStamp = request.BetTime
                }
            };

            return GetBetData(data, request.PlayerCode, transactions, allowPlaceBetWithDifferentPlatformToken);
        }

        #region Private Methods

        private static BetData GetBetData(GameProviderConfigurationData data, string playerCode,
            TransactionData[] transactions, bool allowPlaceBetWithDifferentPlatformToken = false) =>
            new()
            {
                PlayerCode = playerCode,
                PlatformType = data.PlatformType,
                GameProviderId = data.GameProviderId,
                GameProviderConfigurationId = data.GameProviderConfigurationId,
                GameProviderCode = GameProviderCode,
                HasCurrencyFromGameProvider = HasCurrencyFromGameProvider,
                AutoCloseRound = data.AutoCloseRound,
                AllowCancelCancelTransaction = true,
                AllowPlaceBetWithDifferentPlatformToken = allowPlaceBetWithDifferentPlatformToken,
                Transactions = transactions
            };

        private static GameActionType GetGameActionType(ResultType result) =>
            result switch
            {
                ResultType.Tie  => TieBet,
                ResultType.Loss => LoseBet,
                _               => WinBet
            };

        private static BetStatus GetBetStatus(RoundRecord roundData) =>
            roundData.Status == RoundStatus.Closed
                ? roundData.ValidBetAmount == 0 && roundData.WinLoss == 0 ? BetStatus.Void : BetStatus.Settled
                : BetStatus.Running;

        private static (decimal stake, decimal winloss) GetStakeAndWinLoss(BetStatus status, GameActionData ga,
            string externalRoundId, string externalTransactionId)
        {
            if (status == BetStatus.Void)
            {
                return (0, 0);
            }

            var cancelledIds = ga.GameActions
                .Where(gar => gar.Type == CancelTransaction && gar.ReferencedGameActionId.HasValue)
                .Select(gar => new
                {
                    gar.Id,
                    ReferenceId = gar.ReferencedGameActionId
                })
                .ToHashSet();

            var validActions = ga.GameActions
                .Where(gar => cancelledIds.All(c => gar.Id != c.Id && gar.Id != c.ReferenceId))
                .ToHashSet();

            var stake = GetStake(validActions, externalRoundId, externalTransactionId);
            var winloss = GetWinLoss(validActions);

            return (stake, winloss);
        }

        private static decimal GetStake(HashSet<GameActionRecord> validActions, string externalRoundId,
            string externalTransactionId)
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
            validActions
                .Where(ga => ga.Type != PlaceBet)
                .Sum(ga => ga.Amount);

        private static T Response<T>(string playerCode, decimal balance, GameApiError error = GameApiError.Success)
            where T : BaseResponse, new() =>
            new()
            {
                ErrorCode = (int)error,
                ErrorMessage = error.Description(),
                PlayerCode = playerCode,
                Balance = error != GameApiError.Success ? null : balance
            };

        BatchBetData ISBOBETActionModelMapper.Request(Guid gameProviderId,
            Dictionary<string, Guid> playerConfigurationIds, bool autoCloseRound, BatchRequest request) =>
            new()
            {
                ExternalBatchId = request.ExternalBatchId,
                GameProviderId = gameProviderId,
                GameProviderCode = GameProviderCode,
                AutoCloseRound = autoCloseRound,
                Transactions = request.Transactions.Select(t =>
                {
                    playerConfigurationIds.TryGetValue(t.PlayerCode, out var gameProviderConfigurationid);

                    return new BatchTransactionData
                    {
                        PlayerCode = t.PlayerCode,
                        GameProviderConfigurationId = gameProviderConfigurationid,
                        Amount = t.Amount,
                        ExternalRoundId = t.ExternalRoundId,
                        ExternalBetId = t.ExternalBetId,
                        ExternalGameId = t.ExternalGameId,
                        ExternalTransactionId = t.ExternalTransactionId,
                        ExternalTransactionReferenceId = t.ExternalTransactionReferenceId,
                        GameActionType = CancelTransaction,
                        CurrencyCode = t.CurrencyCode,
                        Timestamp = request.Timestamp,
                        Description = t.Description,
                        RoundClosed = t.RoundClosed
                    };
                }).ToArray()
            };

        BatchResponse ISBOBETActionModelMapper.Response(BatchBetResult r) =>
            new()
            {
                ErrorCode = (int)r.Error,
                ErrorMessage = r.ErrorMessage,
                BatchTime = r.BatchTime,
                ExternalBatchId = r.ExternalBatchId,
                IsDuplicate = r.IsDuplicate,
                TransactionsCount = r.TransactionsCount,
                PlayerBalances = r.PlayerBalances,
                Errors = r.Errors?.Select(t => new BatchResponseTransaction
                {
                    PlayerCode = t.PlayerCode,
                    ExternalTransactionId = t.ExternalTransactionId,
                    InternalTransactionId = t.InternalTransactionId,
                    IsDuplicate = t.IsDuplicate,
                    Error = t.Error,
                    ErrorMessage = t.ErrorMessage
                }).ToArray()
            };

        #endregion Private Methods
    }
}