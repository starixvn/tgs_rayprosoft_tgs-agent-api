using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.TimeZone;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations.Calculators;
using TGS.Services.GameOperations.Models.Data;
using TGS.Services.GameOperations.Validators;
using TGS.Services.Operations.Validators;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Services.GameOperations.Data
{
    internal interface IGameActionProcessor
    {
        Task<BetResult> ExecuteAsync(ProcessorData data);
    }

    internal sealed partial class GameActionProcessor : IGameActionProcessor
    {
        private static readonly GameType[] BypassPreviousActionGameTypes = {GameType.Slot};
        private readonly IActionWithinRoundValidator _actionWithinRoundValidator;
        private readonly IAmountValidator _amountValidator;
        private readonly IBetValuesCalculator _betValuesCalculator;
        private readonly IRoundStatusManager _roundStatusManager;
        private readonly IStatusValidator _statusValidator;
        private readonly ITimeZoneManager _timeZoneManager;
        private readonly ITransactionSettingsGenerator _transactionSettingsGenerator;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IValidStatus<Status> _validStatus;

        public GameActionProcessor(
            ITransactionSettingsGenerator transactionSettingsGenerator,
            IAmountValidator amountValidator,
            IStatusValidator statusValidator,
            IActionWithinRoundValidator actionWithinRoundValidator,
            IBetValuesCalculator betValuesCalculator,
            ITimeZoneManager timeZoneManager,
            IRoundStatusManager roundStatusManager,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus)
        {
            _transactionSettingsGenerator = transactionSettingsGenerator;
            _amountValidator = amountValidator;
            _statusValidator = statusValidator;
            _actionWithinRoundValidator = actionWithinRoundValidator;
            _betValuesCalculator = betValuesCalculator;
            _timeZoneManager = timeZoneManager;
            _roundStatusManager = roundStatusManager;
            _validStatus = validStatus;
            _validGameStatus = validGameStatus;
        }

        async Task<BetResult> IGameActionProcessor.ExecuteAsync(ProcessorData data)
        {
            var transactionResults = new List<TransactionResult>();
            var betData = data.BetData;
            var player = data.Player;

            foreach (var trx in betData.Transactions)
            {
                var transactionSettings = _transactionSettingsGenerator.GetTransactionSettings(trx.GameActionType);
                var hasWalletUpdate = transactionSettings.HasWalletUpdate(trx);

                var result = await HandleGameActionAsync(new(trx)
                {
                    Repository = data.Repository,
                    GameProvider = data.GameProvider,
                    GameProviderConfigurationId = betData.GameProviderConfigurationId,
                    BatchId = betData.BatchId,
                    PlatformType = betData.PlatformType,
                    HasWalletUpdate = hasWalletUpdate,
                    MustHaveExistingRound = transactionSettings.MustHaveExistingRound,
                    MustHaveReferencedTransaction = transactionSettings.MustHaveReferencedTransaction,
                    ValidateAmount = transactionSettings.ValidateAmount(trx),
                    PrepareAmount = transactionSettings.GetTransactionAmount,
                    PlayerData = player,
                    GameProviderGames = data.Games,
                    ForceGetPreviousActions = betData.ForceGetPreviousActions,
                    FreeBetAsSingleRound = betData.FreeBetAsSingleRound,
                    AllowCancelCancelTransaction = betData.AllowCancelCancelTransaction,
                    AutoCloseRound = betData.AutoCloseRound
                });

                transactionResults.Add(result.TransactionResult);
            }

            return new()
            {
                Balance = player.Wallet.Balance,
                CurrencyCode = player.Wallet.CurrencyCode,
                Transactions = transactionResults.ToArray(),
                PlatformType = (int)betData.PlatformType,
                Error = TgsApiError.None
            };
        }

        private async Task<GameActionResult> HandleGameActionAsync(GameActionState s)
        {
            ThrowIfHaveNoRequiredIds(s);
            ValidateAmounts(s);

            s = GetGame(s);
            s = await FillRoundDataAsync(s);

            var (st, r) = await GetAndValidatePreviousActionsAsync(s);
            if (r != null)
            {
                s.PlayerData.Wallet.Balance = await GetBalanceAsync(s.Repository, s.PlayerData.Wallet.Id);
                return r;
            }

            s = st;

            ThrowIfReferenceGameActionOrRoundIsNotFound(s);

            s = GetNewRoundStatus(s);

            await ThrowIfStatusesIsInvalidAsync(s);

            var ga = await SaveDatabaseRecordsAsync(s);

            var output = new GameActionResult
            {
                TransactionResult = new()
                {
                    ExternalTransactionId = s.ExternalTransactionId,
                    InternalTransactionId = ga.Id,
                    IsDuplicate = false
                },
                GameAction = ga,
                ReferencedGameAction = s.ReferencedGameAction,
                Game = s.Game
            };

            return output;
        }

        private static async Task<GameActionState> FillRoundDataAsync(GameActionState s)
        {
            var roundData = await GetRoundByExternalRoundIdAsync(s.Repository,
                s.GameProviderConfigurationId, s.PlayerData.Wallet.Id, s.ExternalRoundId, s.GameId);

            if (roundData == null)
            {
                if (s.MustHaveExistingRound)
                {
                    throw new InvalidGameActionException(
                        $"Cannot find expected round with id = {s.ExternalRoundId}",
                        InvalidGameActionErrorCode.RoundNotFound);
                }

                return s;
            }

            s.InternalRoundId = roundData.RoundId;
            s.OldRoundStatus = roundData.Status;

            return s;
        }

        private static GameActionState GetGame(GameActionState s)
        {
            var game =
                s.GameProviderGames.FirstOrDefault(
                    g => g.ExternalId.IgnoreCaseEquals(s.ExternalGameId)
                        && g.PlatformType == s.PlatformType);

            if (game == null)
            {
                throw new InvalidGameActionException(
                    $"Cannot find game '{s.ExternalGameId}' for platform '{s.PlatformType}' of provider config with id={s.GameProviderConfigurationId}",
                    InvalidGameActionErrorCode.GameNotFound);
            }

            s.GameId = game.Id;
            s.Game = game;

            return s;
        }

        private static async Task<(GameActionState, GameActionResult)> GetAndValidatePreviousActionsAsync(
            GameActionState s)
        {
            if (!ShouldGetPreviousActions(s))
            {
                return (s, null);
            }

            var repository = s.Repository;
            var externalRoundId = s.ExternalRoundId;

            // get all transactions belonging to this round
            var actionsData =
                await GetGameActionsInSameRoundAsync(repository, s.GameProvider.Id, s.PlayerData.PlayerCode,
                    externalRoundId, s.GameId);

            if (actionsData.Length == 0 && s.HasExtRefTransactionId)
            {
                await ThrowIfExternalTransactionIdBelongsToDifferentRoundAsync(repository, externalRoundId,
                    s.ExternalTransactionReferenceId);
                return (s, null);
            }

            var (refData, refGameActionInternalId, amount) = GetReferencedActionAndActualAmount(s, actionsData);

            s.Amount = amount;
            s.ReferencedGameActionInternalId = refGameActionInternalId;

            if (actionsData.Length == 0)
            {
                return (s, null);
            }

            var dict = actionsData
                .Select(act => new BetActionData
                {
                    Id = act.GameActionId,
                    ExternalId = act.ExternalTransactionId,
                    ReferenceId = act.ReferenceGameActionId,
                    ExternalRoundId = externalRoundId,
                    ExternalGameActionGroupId = act.ExternalGameActionGroupId,
                    Amount = act.Amount,
                    UnsettledBets = act.UnsettledBets,
                    Turnover = act.Turnover,
                    WinLoss = act.WinLoss,
                    IsClosingRound = act.IsClosingRound,
                    IsExplicitlyClosingRound = act.ClosingRoundExplicitRequest == true,
                    IsExplicitlyNotClosingRound = act.ClosingRoundExplicitRequest == false,
                    Type = act.Type,
                    RecordedOn = act.GameActionRecordedOn,
                    SentOn = act.GameActionSentOn,
                    GameName = s.Game.Name,
                    GameProviderCode = s.GameProvider.Code,
                    BetType = act.BetType,
                    WalletId = act.WalletId,
                    IsCancelled = false
                })
                .ToDictionary(e => e.Id);

            var (actions, refGameAction) = GetPreviousActions(s.GameActionType, refGameActionInternalId, dict);

            s.ReferencedGameAction = refGameAction;
            s.PreviousActions = actions;

            if (s.InternalRoundId == null && refData != null)
            {
                s.InternalRoundId = refData.RoundId;
                s.OldRoundStatus = refData.RoundStatus;
            }

            var r = ValidateReferenceAction(s);
            if (r != null)
            {
                return (s, r);
            }

            s.ValidPreviousActions = GetValidPreviousActions(actions);

            return (s, r);
        }

        private static (IList<BetActionData>, BetActionData) GetPreviousActions(GameActionType currentActionType,
            Guid? refGameActionInternalId, Dictionary<Guid, BetActionData> actionMap)
        {
            var refGameAction = default(BetActionData);
            var actions = new List<BetActionData>();

            foreach (var act in actionMap.Values)
            {
                if (act.ReferenceId.HasValue && actionMap.TryGetValue(act.ReferenceId.Value, out var refBet))
                {
                    act.Reference = refBet;
                    UpdateReferenceGameActionIsCancelled(act.Type, act.Reference);
                }

                if (act.Id == refGameActionInternalId)
                {
                    refGameAction = act;
                    UpdateReferenceGameActionIsCancelled(currentActionType, refGameAction);
                }

                actions.Add(act);
            }

            return (actions.ToArray(), refGameAction);
        }

        private static IList<BetActionData> GetValidPreviousActions(IList<BetActionData> actions)
        {
            var allCancelledIdsInTheRound = actions
                .Where(r => r.Type == CancelTransaction && r.Reference != null)
                .Select(r => r.Reference.Id).ToHashSet();

            return actions.Where(r => !allCancelledIdsInTheRound.Contains(r.Id)).ToArray();
        }

        private static (SameRoundActionData, Guid?, decimal) GetReferencedActionAndActualAmount(GameActionState s,
            SameRoundActionData[] actionsData)
        {
            var amount = s.PrepareAmount(s.Amount);

            if (actionsData.Length == 0)
            {
                return (null, null, amount);
            }

            Guid? refGameActionInternalId = null;
            SameRoundActionData refData = null;

            if (s.HasExtRefTransactionId)
            {
                refData = actionsData.FirstOrDefault(a =>
                    a.ExternalTransactionId.IgnoreCaseEquals(s.ExternalTransactionReferenceId));

                if (refData != null)
                {
                    if (refData.ExternalRoundId.IgnoreCaseNotEquals(s.ExternalRoundId))
                    {
                        throw new InvalidGameActionException(
                            $"Round ID '{s.ExternalRoundId}' of referenced transaction was not as expected.");
                    }

                    refGameActionInternalId = refData.GameActionId;

                    // if this is cancel action, get the amount from the referenced action
                    if (s.GameActionType == CancelTransaction)
                    {
                        amount = s.PrepareAmount(refData.Amount);
                    }
                }
            }

            refData ??= actionsData[0];

            return (refData, refGameActionInternalId, amount);
        }

        /// <summary>
        ///     get previous actions:
        ///     1. ForceGetPreviousActions = true
        ///     2. for settlement transactions
        ///     3. place bet is not slot games
        ///     4. not lock amount action
        /// </summary>
        private static bool ShouldGetPreviousActions(GameActionState s) =>
            s.ForceGetPreviousActions ||
            s.GameActionType.IsNotAllOf(LockAmount, PlaceBet) ||
            !BypassPreviousActionGameTypes.Contains(s.Game.Type);

        private static GameActionResult GetDuplicateTransactionResult(Guid gameActionId,
            string externalTransactionId) =>
            new()
            {
                TransactionResult = new()
                {
                    ExternalTransactionId = externalTransactionId,
                    InternalTransactionId = gameActionId,
                    IsDuplicate = true
                }
            };

        private static void UpdateReferenceGameActionIsCancelled(GameActionType currentActionType,
            BetActionData reference)
        {
            if (currentActionType != CancelTransaction)
            {
                return;
            }

            reference.IsCancelled = true;

            //un-cancel transaction
            if (reference.Type == CancelTransaction)
            {
                reference.Reference.IsCancelled = false;
            }
        }

        private GameActionState GetNewRoundStatus(GameActionState s)
        {
            s.NewRoundStatus = _roundStatusManager.GetStatus(new()
            {
                AutoCloseRound = s.AutoCloseRound,
                RequestToCloseRound = s.RoundClosed,
                RoundId = s.InternalRoundId ?? Guid.Empty,
                OldStatus = s.OldRoundStatus,
                GameActionType = s.GameActionType,
                ReferencedGameAction = s.ReferencedGameAction,
                PreviousActions = s.PreviousActions,
                ValidPreviousActions = s.ValidPreviousActions
            });

            return s;
        }

        private async Task<GameAction> SaveDatabaseRecordsAsync(GameActionState s)
        {
            var current = new BetActionData
            {
                Id = GuidExtensions.NewSequentialGuid(),
                ExternalId = s.ExternalTransactionId,
                ExternalRoundId = s.ExternalRoundId,
                ReferenceId = s.ReferencedGameActionInternalId,
                Reference = s.ReferencedGameAction,
                Amount = s.Amount,
                Type = s.GameActionType,
                IsClosingRound = s.NewRoundStatus == RoundStatus.Closed,
                IsExplicitlyClosingRound = s.RoundClosed == true,
                IsExplicitlyNotClosingRound = s.RoundClosed == false,
                GameName = s.Game.Name,
                GameProviderCode = s.GameProvider.Code,
                BetType = s.BetType,
                AllowCancelCancelTransaction = s.AllowCancelCancelTransaction
            };

            var error = _actionWithinRoundValidator.FindError(current, s.PreviousActions);
            if (error != null)
            {
                throw error;
            }

            var betValues = _betValuesCalculator.Calculate(s.Game.HasTurnoverOnTieBet, current, s.PreviousActions);

            s.BetValueResult = betValues;

            current.Turnover = betValues.Turnover;
            current.WinLoss = betValues.WinLoss;
            current.UnsettledBets = betValues.UnsettledBets;

            s.PlayerData.Wallet.Balance = await GetOrUpdateBalanceAsync(s);
            s.InternalRoundId = CreateOrUpdateRound(s, current);

            var ga = CreateGameAction(s, current);

            await s.Repository.SaveChangesAsync();

            return ga;
        }

        private static async Task<decimal> GetOrUpdateBalanceAsync(GameActionState s)
        {
            var repository = s.Repository;
            var walletId = s.PlayerData.Wallet.Id;

            decimal balance;
            if (s.HasWalletUpdate && s.Amount != 0)
            {
                balance =
                    await AtomicUpdateBalanceAsync(repository, s.PlayerData.PlayerCode, walletId, s.Amount, s.UtcNow);

                if (balance < 0 && s.GameActionType.IsOneOf(PlaceBet, LockAmount))
                {
                    throw new PlayerHasInsufficientFundsException();
                }
            }
            else
            {
                balance = await GetBalanceAsync(repository, walletId);
            }

            return balance;
        }

        private Guid CreateOrUpdateRound(GameActionState s, BetActionData current) =>
            s.InternalRoundId == null ? CreateNewRoundAndGetId(s, current) : UpdatedRound(s, current);

        private Guid CreateNewRoundAndGetId(GameActionState s, BetActionData current)
        {
            var repository = s.Repository;
            var roundId = GuidExtensions.NewSequentialGuid();

            var round = new Round
            {
                Id = roundId,
                ExternalRoundId = s.ExternalRoundId,
                GameProviderId = s.GameProvider.Id,
                PlayerCode = s.PlayerData.PlayerCode,
                GameProviderConfigurationId = s.GameProviderConfigurationId,
                WalletId = s.PlayerData.Wallet.Id,
                GameId = s.GameId,
                PlatformType = s.PlatformType,
                CurrencyCode = s.PlayerData.Wallet.CurrencyCode,
                BrandId = s.PlayerData.BrandId,
                CreatedOn = s.UtcNow,
                UpdatedOn = s.UtcNow,
                Status = RoundStatus.Open,
                ClientStatus = ClientRoundStatus.Open,
                ExternalCreatedOn = s.Timestamp,
                BetAmount = s.BetValueResult.BetAmount
            };

            if (s.NewRoundStatus == RoundStatus.Closed)
            {
                FillClosedRoundBetValues(s, current, round);
            }

            repository.Rounds.Add(round);

            if (s.RoundDetail.IsNotNullOrEmpty())
            {
                repository.RoundDetails.Add(new()
                {
                    RoundId = roundId,
                    AdditionalData = s.RoundDetail
                });
            }

            return roundId;
        }

        private Guid UpdatedRound(GameActionState s, BetActionData current)
        {
            var repository = s.Repository;

            var roundId = s.InternalRoundId.Value;
            var round = repository.Rounds.Find(roundId);

            round.BetAmount = s.BetValueResult.BetAmount;
            round.Commission = s.Commission;
            round.UpdatedOn = s.UtcNow;

            if (s.OldRoundStatus == RoundStatus.Closed && s.NewRoundStatus == RoundStatus.Open)
            {
                round.Status = s.NewRoundStatus;
                round.ClientStatus = ClientRoundStatus.Open;
            }
            else if (s.NewRoundStatus == RoundStatus.Closed)
            {
                FillClosedRoundBetValues(s, current, round);
            }

            repository.Rounds.Update(round);

            if (s.RoundDetail.IsNullOrEmpty())
            {
                return roundId;
            }

            var detail = repository.RoundDetails.Find(roundId);

            if (detail == null)
            {
                repository.RoundDetails.Add(new()
                {
                    RoundId = roundId,
                    AdditionalData = s.RoundDetail
                });
            }
            else if (detail.AdditionalData != s.RoundDetail)
            {
                detail.AdditionalData = s.RoundDetail;
                repository.RoundDetails.Update(detail);
            }

            return roundId;
        }

        private static GameAction CreateGameAction(GameActionState s, BetActionData current)
        {
            var player = s.PlayerData;

            var externalGameActionGroupId = s.ReferencedGameAction != null && !s.HasExternalGameActionGroupId
                ? s.ReferencedGameAction.ExternalGameActionGroupId
                : s.ExternalGameActionGroupId ?? s.ExternalRoundId;

            var ga = new GameAction
            {
                Id = current.Id,
                ExternalBetId = s.ExternalBetId,
                ExternalTransactionId = s.ExternalTransactionId,
                GameActionBatchId = s.BatchId,
                ExternalGameActionGroupId = externalGameActionGroupId,
                ReferencedGameActionId = s.ReferencedGameActionInternalId,
                Type = s.GameActionType,
                RoundId = s.InternalRoundId.Value,
                RecordedOn = AdjustRecordedOnIfNeeded(s),
                SentOn = s.Timestamp,
                PlayerIp = player.PlayerIp,
                Turnover = current.Turnover,
                WinLoss = current.WinLoss,
                UnsettledBets = current.UnsettledBets,
                IsClosingRound = current.IsClosingRound,
                ClosingRoundExplicitRequest =
                    current.IsExplicitlyClosingRound    ? true :
                    current.IsExplicitlyNotClosingRound ? false : (bool?)null,
                Amount = s.Amount,
                Balance = player.Wallet.Balance,
                BetType = s.BetType,
                Description = s.Description
            };

            s.Repository.GameActions.Add(ga);

            return ga;
        }

        private static DateTimeOffset AdjustRecordedOnIfNeeded(GameActionState s)
        {
            var now = DateTimeOffset.UtcNow;

            if (s.PreviousActions == null)
            {
                return now;
            }

            var previousRecordedOn = s.PreviousActions
                .OrderByDescending(x => x.RecordedOn)
                .Select(x => x.RecordedOn)
                .FirstOrDefault();

            // due to multiple servers, if current server time is earlier than the latest timestamp,
            // take the lasted timestamp and add 50 milliseconds as current timestamp.
            return now > previousRecordedOn ? now : previousRecordedOn.AddMilliseconds(50);
        }

        private void FillClosedRoundBetValues(GameActionState s, BetActionData current, Round round)
        {
            var hasFreeBet = current.Type == FreeBet ||
                s.PreviousActions.Any(act => act.Type == FreeBet);

            var clientStatus = _roundStatusManager.GetClientRoundStatus(s.NewRoundStatus, s.BetValueResult, hasFreeBet);

            round.Status = s.NewRoundStatus;
            round.ClientStatus = clientStatus;
            round.ClosedOn = s.UtcNow;
            round.ExternalClosedOn = s.Timestamp;
            round.WinLoss = s.BetValueResult.WinLoss;
            round.TieBetAdjustment = s.BetValueResult.TieBetAdjustment;
            round.SettleAmount = s.BetValueResult.SettleAmount;
            round.Turnover = s.BetValueResult.Turnover;
        }
    }
}