using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures.Extensions;
using TGS.Services.Operations.Models;
using TGS.Services.Operations.Models.Data;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Services.Operations.Bets
{
    public sealed partial class GameActionDataOperations : IGameActionDataOperations
    {
        private async Task<BetResult> ExecuteGameActionAsync(BetData data)
        {
            if (data?.Transactions == null || data.Transactions.Length == 0)
            {
                throw new InvalidOperationException("No transactions in the request");
            }

            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(data.PlayerCode);
            ThrowIfPlayerDisableToPlaceBet(data.Transactions, cachedPlayer.Status);

            // TODO: validate if place bet cap amount is exceed

            var cachedLicensee = await _licenseeOperations.GetCachedLicenseeAsync(cachedPlayer.LicenseeId);

            var playerRelatedData = new PlayerRelatedData
            {
                Wallet = new WalletData
                {
                    Id = cachedPlayer.DefaultWalletId,
                    CurrencyCode = cachedPlayer.DefaultWalletCurrencyCode,
                    ExternalCurrencyCode = cachedPlayer.ExternalCurrencyCode
                },
                PlayerCode = data.PlayerCode,
                LicenseeId = cachedPlayer.LicenseeId,
                LicenseeCode = cachedLicensee.Code,
                LicenseeName = cachedLicensee.Name,
                Status = cachedPlayer.Status,
                PlayerType = cachedPlayer.Type,
                PlayerUsername = cachedPlayer.Username,
                PlayerExternalId = cachedPlayer.ExternalId,
                ExternalCurrencyCode = cachedPlayer.ExternalCurrencyCode,
                LicenseeTimeZoneId = cachedLicensee.TimeZoneId
                //PlayerIp = data..Ip
            };

            var cacheGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(data.GameProviderId);
            var games = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId);

            var externalTransactionIds = data.Transactions.Select(t => t.ExternalTransactionId).JoinAsStringBySemicolon();
            var transactionResult = await _transactionManager.ExecuteTransactionAsync(
                _tgsRepoFactory,
                IsolationLevel.RepeatableRead,
                externalTransactionIds,
                execute: async (r, trans) => await ProcessGameActionsAsync(r, data, playerRelatedData, cacheGameProvider, games));

            return transactionResult;
        }

        private async Task<BetResult> ProcessGameActionsAsync(ITgsRepository repo, BetData data, PlayerRelatedData player, CachedGameProvider gameProvider, CachedGame[] games)
        {
            var transactionResults = new List<TransactionResult>();

            foreach (var transaction in data.Transactions)
            {
                var transactionSettings = _transactionSettingsGenerator.GetTransactionSettings(transaction.GameActionType);
                var hasWalletUpdate = transactionSettings.HasWalletUpdate(transaction);

                var result = await HandleGameActionAsync(new GameActionState(transaction)
                {
                    Repo = repo,
                    GameProvider = gameProvider,
                    GameProviderConfigurationId = data.GameProviderConfigurationId,
                    BatchId = data.BatchId,
                    PlatformType = data.PlatformType ?? PlatformType.Desktop,
                    HasWalletUpdate = hasWalletUpdate,
                    MustHaveExistingRound = transactionSettings.MustHaveExistingRound,
                    MustHaveReferencedTransaction = transactionSettings.MustHaveReferencedTransaction,
                    ValidateAmount = transactionSettings.ValidateAmount(transaction),
                    PrepareAmount = transactionSettings.GetTransactionAmount,
                    PlayerData = player,
                    GameProviderGames = games,
                    ForceGetPreviousActions = data.ForceGetPreviousActions,
                    FreeBetAsSingleRound = data.FreeBetAsSingleRound
                });

                transactionResults.Add(result.TransactionResult);
            }            

            return new BetResult
            {
                Balance = player.Wallet.Balance,
                CurrencyCode = player.Wallet.CurrencyCode,
                Transactions = transactionResults.ToArray(),
                PlatformType = (int) data.PlatformType.Value,
                Error = TgsApiError.None,
            };
        }

        private async Task<GameActionResult> HandleGameActionAsync(GameActionState s)
        {
            ThrowIfHaveNoRequiredIds(s);
            ValidateAmounts(s);

            s = await FillRoundDataAsync(s);
            s = GetGame(s);
            var (st, r) = await GetAndValidatePreviousActionsAsync(s);
            if (r != null)
                return r;

            s = st;

            ThrowIfReferenceGameActionOrRoundIsNotFound(s);

            s = FillEmptyData(s);

            await ThrowIfStatusesIsInvalidAsync(s);

            var ga = await SaveDatabaseRecordsAsync(s);

            var output = new GameActionResult
            {
                TransactionResult = new TransactionResult
                {
                    ExternalTransactionId = s.ExternalTransactionId,
                    InternalTransactionId = ga.Id,
                    IsDuplicate = false,
                },
                GameAction = ga,
                ReferencedGameAction = s.ReferencedGameAction,
                Game = s.Game
            };

            return output;
        }

        private async Task<GameActionState> FillRoundDataAsync(GameActionState s)
        {
            var roundData = await GetRoundByExternalRoundIdAsync(s.Repo, s.GameProviderConfigurationId, s.PlayerData.Wallet.Id, s.ExternalRoundId);
            if (s.MustHaveExistingRound && roundData == null)
            {
                throw new InvalidGameActionException(
                    $"Cannot find expected round with id = {s.ExternalRoundId}",
                    InvalidGameActionErrorCode.RoundNotFound);
            }

            s.InternalRoundId = roundData.RoundId;
            s.OldRoundStatus = roundData.Status;

            return s;
        }

        private GameActionState GetGame(GameActionState s)
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

        private async Task<(GameActionState, GameActionResult)> GetAndValidatePreviousActionsAsync(GameActionState s)
        {
            if (!ShouldGetPreviousActions(s))
            {
                return (s, null);
            }

            var repo = s.Repo;
            var gameProviderConfigurationId = s.GameProviderConfigurationId;
            var walletId = s.PlayerData.Wallet.Id;
            var externalRoundId = s.ExternalRoundId;

            // get all transactions belonging to this round
            var actionsData = await GetGameActionsInSameRoundAsync(repo, gameProviderConfigurationId, walletId, externalRoundId);

            if (actionsData.Length == 0 && s.HasExtRefTransactionId)
            {
                await ThrowIfExternalTransactionIdBelongsToDifferentRoundAsync(repo, externalRoundId, s.ExternalTransactionReferenceId);
                return (s, null);
            }

            var (refData, refGameActionInternalId, amount) = GetReferencedActionAndActualAmount(s, actionsData);

            var dict = actionsData
                            .Select(act => new BetActionData
                            {
                                Id = act.GameActionId,
                                ExternalId = act.ExternalTransactionId,
                                ReferenceId = act.ReferenceGameActionId,
                                ExternalRoundId = externalRoundId,
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

            var (actions, refGameAction) = GetPreviousActions(refGameActionInternalId, dict);

            s.Amount = amount;
            s.ReferencedGameActionInternalId = refGameActionInternalId;
            s.ReferencedGameAction = refGameAction;
            s.PreviousActions = actions;

            if (s.InternalRoundId == null)
            {
                s.InternalRoundId = refData.RoundId;
                s.OldRoundStatus = refData.RoundStatus;
            }

            var r = ValidateReferenceAction(s);

            s.ValidPreviousActions = GetValidatePreviousActions(actions);

            return (s, r);
        }

        private static (IList<BetActionData>, BetActionData) GetPreviousActions(Guid? refGameActionInternalId, Dictionary<Guid, BetActionData> actionMap)
        {
            BetActionData refGameAction = default;
            List<BetActionData> actions = new();

            foreach (var act in actionMap.Values)
            {
                if (act.ReferenceId.HasValue && actionMap.TryGetValue(act.ReferenceId.Value, out var refBet))
                {
                    var isCancelled = GetGameActionIsCancelled(act.Type, false);
                    act.Reference = refBet;
                    act.Reference.IsCancelled = isCancelled;
                }

                if (act.Id == refGameActionInternalId)
                {
                    var isCancelled = GetGameActionIsCancelled(act.Type, true);
                    act.Reference.Reference.IsCancelled = isCancelled;

                    refGameAction = act;
                }

                actions.Add(act);
            }

            return (actions.AsReadOnly(), refGameAction);
        }

        private IList<BetActionData> GetValidatePreviousActions(IList<BetActionData> actions)
        {
            var allCancelledIdsInTheRound = actions
                .Where(r => r.Type == CancelTransaction && r.Reference != null)
                .Select(r => r.Reference.Id).ToHashSet();

            return actions.Where(r => !allCancelledIdsInTheRound.Contains(r.Id)).ToList().AsReadOnly();
        }

        private static (SameRoundActionData, Guid?, decimal) GetReferencedActionAndActualAmount(GameActionState s, SameRoundActionData[] actionsData)
        {
            var amount = s.PrepareAmount(s.Amount);
            Guid? refGameActionInternalId = null;
            SameRoundActionData refData = null;

            if (s.HasExtRefTransactionId)
            {
                refData = actionsData.FirstOrDefault(a => a.ExternalTransactionId.IgnoreCaseEquals(s.ExternalTransactionReferenceId));

                if (refData != null)
                {
                    if (refData.ExternalRoundId.IgnoreCaseNotEquals(s.ExternalRoundId))
                    {
                        throw new InvalidGameActionException($"Round ID '{s.ExternalRoundId}' of referenced transaction was not as expected.");
                    }

                    refGameActionInternalId = refData.GameActionId;

                    // if this is cancel action, get the amount from the referenced action
                    if (s.GameActionType == CancelTransaction)
                    {
                        amount = s.PrepareAmount(refData.Amount);
                    }
                }
            }

            if (refData == null)
            {
                refData = actionsData[0];
            }

            return (refData, refGameActionInternalId, amount);
        }

        private static bool ShouldGetPreviousActions(GameActionState s)
        {
            // get previous actions:
            // 1. ForceGetPreviousActions = true
            // 2. for settlement transactions
            // 3. place bet is not slot games
            // 4. not lock amount action
            return s.ForceGetPreviousActions ||
                   s.GameActionType.IsNotAllOf(LockAmount, PlaceBet) ||
                   !BypassPreviousActionGameTypes.Contains(s.Game.Type);
        }

        private static bool GetGameActionIsCancelled(GameActionType type, bool isReference)
        {
            if (type != CancelTransaction)
            {
                return false;
            }

            // for un-cancel transaction
            if (isReference)
            {
                return false;
            }

            return true;
        }

        private static GameActionResult GetDuplicateTransactionResult(Guid gameActionId, string externalTransactionId)
        {
            return new()
            {
                TransactionResult = new()
                {
                    ExternalTransactionId = externalTransactionId,
                    InternalTransactionId = gameActionId,
                    IsDuplicate = true
                }
            };
        }

        private GameActionState FillEmptyData(GameActionState s)
        {
            s.LicenseeNow = _timeZoneManager.ToLocal(s.PlayerData.LicenseeTimeZoneId, DateTimeOffset.UtcNow);
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

        private async Task<GameActionData> SaveDatabaseRecordsAsync(GameActionState s)
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
                BetType = s.BetType
            };

            var error = _actionWithinRoundValidator.FindError(current, s.PreviousActions);
            if (error != null)
                throw error;

            var betValues = _betValuesCalculator.Calculate(s.Game.HasTurnoverOnTieBet, current, s.PreviousActions);

            s.BetValueResult = betValues;

            current.Turnover = betValues.Turnover;
            current.WinLoss = betValues.WinLoss;
            current.UnsettledBets = betValues.UnsettledBets;

            s.PlayerData.Wallet.Balance = await GetOrUpdateBalanceAsync(s);
            s.InternalRoundId = await CreateOrUpdateRoundAsync(s);

            var ga = await CreateGameActionAsync(s, current);

            return ga;
        }

        private Task<decimal> GetOrUpdateBalanceAsync(GameActionState s)
        {
            var repo = s.Repo;
            var walletId = s.PlayerData.Wallet.Id;

            if (s.HasWalletUpdate && s.Amount != 0)
            {
                return AtomicUpdateBalanceAsync(repo, s.PlayerData.PlayerCode, walletId, s.Amount, s.LicenseeNow);
            }

            return GetBalanceAsync(repo, walletId);
        }

        private async Task<Guid> CreateOrUpdateRoundAsync(GameActionState s)
        {
            if (s.InternalRoundId == null)
            {
                return await CreateNewRoundAndGetIdAsync(s);
            }

            await UpdatedRoundAsync(s);
            return s.InternalRoundId.Value;
        }

        private async Task<Guid> CreateNewRoundAndGetIdAsync(GameActionState s)
        {
            var roundData = new RoundData
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderConfigurationId = s.GameProviderConfigurationId,
                ExternalRoundId = s.ExternalRoundId,
                GameId = s.GameId,
                GameName = s.Game.Name,
                GameType = s.Game.Type,
                GameProviderId = s.GameProvider.Id,
                GameProviderCode = s.GameProvider.Code,
                GameProviderName = s.GameProvider.Name,
                PlayerCode = s.PlayerData.PlayerCode,
                PlatformType = s.PlatformType,
                WalletId = s.PlayerData.Wallet.Id,
                CurrencyCode = s.PlayerData.Wallet.CurrencyCode,
                LicenseeId = s.PlayerData.LicenseeId,
                LicenseeCode = s.PlayerData.LicenseeCode,
                LicenseeName = s.PlayerData.LicenseeName,
                CreatedOn = s.LicenseeNow,
                UpdatedOn = s.LicenseeNow,
                ClosedOn = s.NewRoundStatus == RoundStatus.Closed ? s.LicenseeNow : default(DateTimeOffset?),
                Status = s.NewRoundStatus == RoundStatus.Closed ? RoundStatus.Closed : RoundStatus.Open,
                RoundDetail = s.RoundDetail,
                ExternalCreatedOn = s.Timestamp,
                ExternalClosedOn = s.NewRoundStatus == RoundStatus.Closed ? s.Timestamp : default(DateTimeOffset?),
            };

            await InsertNewRoundAsync(s.Repo, roundData);

            return roundData.Id;
        }

        private async Task UpdatedRoundAsync(GameActionState s)
        {
            var repo = s.Repo;

            var roundData = new RoundData
            {
                Id = s.InternalRoundId.Value,
                RoundDetail = s.RoundDetail,
                Commission = s.Commission,
                UpdatedOn = s.LicenseeNow,
            };

            if (s.OldRoundStatus == RoundStatus.Closed && s.NewRoundStatus == RoundStatus.Open)
            {
                roundData = roundData with
                {
                    Status = s.NewRoundStatus,
                    IsReopen = true
                };
            }
            else if (s.NewRoundStatus == RoundStatus.Closed)
            {
                roundData = roundData with
                {
                    Status = s.NewRoundStatus,
                    ClosedOn = s.LicenseeNow,
                    ExternalClosedOn = s.Timestamp,
                    BetAmount = s.BetValueResult.BetAmount,
                    WinLoss = s.BetValueResult.WinLoss,
                    TieBetAdjustment = s.BetValueResult.TieBetAdjustment,
                    IsClosed = true
                };
            }

            await UpdateRoundAsync(repo, roundData);
        }

        private async Task<GameActionData> CreateGameActionAsync(GameActionState s, BetActionData current)
        {
            var player = s.PlayerData;

            var gameActionData = new GameActionData
            {
                Id = current.Id,
                ExternalBetId = s.ExternalBetId,
                ExternalTransactionId = s.ExternalTransactionId,
                GameProviderConfigurationId = s.GameProviderConfigurationId,
                GameActionBatchId = s.BatchId,
                ExternalGameActionGroupId = s.ExternalGameActionGroupId,
                ReferencedGameActionId = s.ReferencedGameActionInternalId,
                WalletId = player.Wallet.Id,
                Type = s.GameActionType,
                RoundId = s.InternalRoundId.Value,
                RecordedOn = AdjustRecordedOnIfNeeded(s),
                SentOn = s.Timestamp,
                PlayerCode = player.PlayerCode,
                PlayerIp = player.PlayerIp,
                Turnover = current.Turnover,
                WinLoss = current.WinLoss,
                UnsettledBets = current.UnsettledBets,
                IsClosingRound = current.IsClosingRound,
                ClosingRoundExplicitRequest = current.IsExplicitlyClosingRound ? true :
                    current.IsExplicitlyNotClosingRound ? false : (bool?)null,
                LicenseeId = player.LicenseeId,
                Amount = s.Amount,
                Balance = player.Wallet.Balance,
                BetType = s.BetType,
                Description = s.Description,
            };

            await InsertGameActionAsync(s.Repo, gameActionData);

            return gameActionData;
        }

        private DateTimeOffset AdjustRecordedOnIfNeeded(GameActionState s)
        {
            throw new NotImplementedException();
        }
    }
}