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

namespace TGS.Services.Operations
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

            // TODO: validate if place bet cap amount is exceed

            var playerRelatedData = new PlayerRelatedData
            {
                Wallet = new WalletData
                {
                    Id = cachedPlayer.DefaultWalletId,
                    CurrencyCode = cachedPlayer.DefaultWalletCurrencyCode,
                    ExternalCurrencyCode = cachedPlayer.ExternalCurrencyCode
                },
                PlayerCode = data.PlayerCode,
                BrandId = cachedPlayer.BrandId,
                BrandCode = cachedPlayer.BrandCode,
                Status = cachedPlayer.Status,
                PlayerType = cachedPlayer.Type,
                PlayerUsername = cachedPlayer.Username,
                PlayerExternalId = cachedPlayer.ExternalId,
                ExternalCurrencyCode = cachedPlayer.ExternalCurrencyCode,
                //PlayerIp = data..Ip
            };

            var games = await _gameProviderOperations.GetCachedGamesAsync(data.GameProviderId);

            var externalTransactionIds = data.Transactions.Select(t => t.ExternalTransactionId).JoinAsStringBySemicolon();
            var transactionResult = await _transactionManager.ExecuteTransactionAsync(
                _tgsRepoFactory,
                IsolationLevel.RepeatableRead,
                externalTransactionIds,
                execute: async (r, trans) => await ProcessGameActionsAsync(r, data, playerRelatedData, games));

            return transactionResult;
        }

        private async Task<BetResult> ProcessGameActionsAsync(ITgsRepository repo, BetData data, PlayerRelatedData player, CachedGame[] games)
        {
            var hasChanges = false;
            var transactionResults = new List<TransactionResult>();

            foreach (var transaction in data.Transactions)
            {
                var transactionSettings = _transactionSettingsGenerator.GetTransactionSettings(transaction.GameActionType);
                var hasWalletUpdate = transactionSettings.HasWalletUpdate(transaction);

                var result = await HandleGameActionAsync(new GameActionState(transaction)
                {
                    Repo = repo,
                    GameProviderId = data.GameProviderId,
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
            }

            return new BetResult { };
        }

        private async Task<GameActionResult> HandleGameActionAsync(GameActionState s)
        {
            ThrowIfHaveNoRequiredIds(s);
            ValidateAmounts(s);

            s = await FillRoundDataAsync(s);
            s = GetGame(s);
            s = await GetPreviousGameActionsAsync(s);

            return null;
        }

        private async Task<GameActionState> FillRoundDataAsync(GameActionState s)
        {
            var roundData = await GetRoundByExternalRoundIdAsync(s.Repo, s.GameProviderConfigurationId, s.PlayerData.Wallet.Id, s.ExternalRoundId);
            if (s.MustHaveExistingRound && roundData == null)
            {
                throw new InvalidGameActionException($"Cannot find expected round with id={s.ExternalRoundId}");
            }

            return s with { InternalRoundId = roundData.RoundId, OldRoundStatus = roundData.Status };
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

            return s with { GameId = game.Id, Game = game };
        }

        private async Task<GameActionState> GetPreviousGameActionsAsync(GameActionState s)
        {
            if (!ShouldGetPreviousActions(s))
                return s;

            var repo = s.Repo;
            var gameProviderConfigurationId = s.GameProviderConfigurationId;
            var walletId = s.PlayerData.Wallet.Id;
            var externalRoundId = s.ExternalRoundId;

            // get all transactions belonging to this round
            var actionsData = await GetGameActionsInSameRoundAsync(repo, gameProviderConfigurationId, walletId, externalRoundId);

            if (actionsData.Length == 0 && s.HasExtRefTransactionId)
            {
                await ThrowIfExternalTransactionIdBelongsToDifferentRoundAsync(repo, externalRoundId, s.ExternalTransactionReferenceId);
                return s;
            }

            var (refData, refGameActionInternalId, amount) = GetReferencedActionAndActualAmount(s, actionsData);
            var game = GetGameViaReferencedGameAction(s, refData);

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
                                GameName = game.Name,
                                GameProviderCode = s.GameProviderCode,
                                BetType = act.BetType,
                                BetId = act.BetId,
                                WalletId = act.WalletId,
                                IsCancelled = false
                            })
                            .ToDictionary(e => e.Id);

            BetActionData refGameAction = default;
            List<BetActionData> previousActions = new();

            foreach (var act in dict.Values)
            {
                BetActionData data = act;
                if (act.ReferenceId.HasValue && dict.TryGetValue(act.ReferenceId.Value, out var refBet))
                {
                    var isCancelled = GetGameActionIsCancelled(act.Type, false);
                    data = data with
                    {
                        Reference = refBet with { IsCancelled = isCancelled }
                    };
                }

                if (act.Id == refGameActionInternalId)
                {
                    var isCancelled = GetGameActionIsCancelled(act.Type, true);
                    data = data with
                    {
                        Reference = data.Reference with { IsCancelled = isCancelled }
                    };

                    refGameAction = data;
                }

                previousActions.Add(data);
            }

            return s with
            {
                Amount = amount,
                RefGameActionInternalId = refGameActionInternalId,
                GameId = refData.GameId,
                Game = game,
                InternalRoundId = refData.RoundId,
                BetId = refData.BetId,
                OldRoundStatus = refData.RoundStatus,
                PreviousActions = previousActions.AsReadOnly()
            };
        }

        private static (SameRoundActionData refData, Guid? refGameActionInternalId, decimal amount) GetReferencedActionAndActualAmount(GameActionState s, SameRoundActionData[] actionsData)
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
                    if (s.GameActionType == GameActionType.CancelTransaction)
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
                   s.GameActionType.IsNotAllOf(GameActionType.LockAmount, GameActionType.PlaceBet) ||
                   !BypassPreviousActionGameTypes.Contains(s.Game.Type);
        }

        private static CachedGame GetGameViaReferencedGameAction(GameActionState s, SameRoundActionData refData)
        {
            return s.Game ?? s.GameProviderGames.FirstOrDefault(g => g.Id == refData.GameId);
        }

        private static bool GetGameActionIsCancelled(GameActionType type, bool isReference)
        {
            if (type != GameActionType.CancelTransaction)
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
    }
}