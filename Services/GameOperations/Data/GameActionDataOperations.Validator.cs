using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations.Models.Data;
using static TGS.Core.Models.Enums.GameActionType;
using GameActionResult = TGS.Services.GameOperations.Models.Data.GameActionResult;

namespace TGS.Services.GameOperations.Data
{
    internal sealed partial class GameActionDataOperations
    {        
        private void ThrowIfPlayerDisableToPlaceBet(TransactionData[] transactions, PlayerStatus playerStatus)
        {
            if (transactions.Any(t => t.GameActionType.IsOneOf(LockAmount, PlaceBet)))
            {
                _statusValidator.ThrowIfPlayerDisabledToPlaceBet(playerStatus);
            }
        }

        private void ValidateAmounts(GameActionState s)
        {
            s.ValidateAmount(nameof(s.Amount), s.Amount);

            if (s.Commission.HasValue)
            {
                _amountValidator.ThrowIfAmountIsNegative(nameof(s.Commission), s.Commission.Value);
            }
        }

        private static void ThrowIfHaveNoRequiredIds(GameActionState s)
        {
            if (s.MustHaveReferencedTransaction && !s.HasExtRefTransactionId)
            {
                throw new InvalidGameActionException(
                    "Missing External Transaction Reference Id",
                    InvalidGameActionErrorCode.TransactionBeingReferencedNotFound);
            }

            if (s.MustHaveExistingRound && !s.HasExtRoundId)
            {
                throw new InvalidGameActionException("Missing External Round Id", InvalidGameActionErrorCode.RoundNotFound);
            }

            if (!s.HasExtGameId)
            {
                throw new InvalidGameActionException(
                    $"Missing game external id for '{s.PlatformType}' of provider config with id={s.GameProviderConfigurationId}",
                    InvalidGameActionErrorCode.GameNotFound);
            }

            if (s.GameActionType.IsNotAllOf(LockAmount, PlaceBet, UnlockAmount) && s.ExternalBetId.IsNullOrEmpty())
            {
                throw new InvalidGameActionException("Missing external bet ID.");
            }
        }

        private static async Task ThrowIfExternalTransactionIdBelongsToDifferentRoundAsync(Db db, string externalRoundId, string externalTransactionId)
        {
            var foundExternalRoundId = await GetExternalRoundIdAsync(db, externalTransactionId);
            if (foundExternalRoundId.IsNotNullOrEmpty() && foundExternalRoundId.IgnoreCaseNotEquals(externalTransactionId))
            {
                throw new InvalidGameActionException(
                       $"Expected the reference transaction '{externalTransactionId}' to belong to round '{externalRoundId}' but it belongs to '{foundExternalRoundId}'",
                       InvalidGameActionErrorCode.TransactionNotFound);
            }
        }

        private static GameActionResult ValidateReferenceAction(GameActionState s)
        {
            var action = s.PreviousActions?.FirstOrDefault(pa => pa.ExternalId == s.ExternalTransactionId);

            if (action == null)
            {
                return null;
            }

            if (s.PlayerData.Wallet.Id != action.WalletId)
            {
                throw new InvalidGameActionException(
                    $"External transaction ID: {s.ExternalTransactionId} was already used for another player.",
                    InvalidGameActionErrorCode.Duplicate);
            }

            return GetDuplicateTransactionResult(action.Id, s.ExternalTransactionId);
        }

        private static void ThrowIfReferenceGameActionOrRoundIsNotFound(GameActionState s)
        {
            if (s.HasExtRefTransactionId && s.ReferencedGameAction == null)
            {
                if (s.GameActionType == CancelTransaction)
                {
                    throw new InvalidGameActionException(
                        $"Transaction being cancelled not found, external transaction ID: {s.ExternalTransactionReferenceId}",
                        InvalidGameActionErrorCode.TransactionBeingCancelledNotFound);
                }

                throw new InvalidGameActionException(
                    $"Transaction being referenced not found, transaction ID: {s.ExternalTransactionReferenceId} for game provider: {s.GameProvider.Code}",
                    InvalidGameActionErrorCode.TransactionBeingReferencedNotFound);
            }

            if (s.MustHaveExistingRound && s.InternalRoundId == null)
            {
                throw new InvalidGameActionException(
                    $"Cannot find expected round with id={s.ExternalRoundId}",
                    InvalidGameActionErrorCode.RoundNotFound);
            }
        }

        private async Task ThrowIfStatusesIsInvalidAsync(GameActionState s)
        {
            var playerType = s.PlayerData.PlayerType;
            var gameActionType = s.GameActionType;
            var gameProviderId = s.GameProvider.Id;
            var gameId = s.GameId;

            var status = _validStatus.GetStatus(playerType, gameActionType);
            var gameStatus = _validGameStatus.GetStatus(playerType, gameActionType);

            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(s.PlayerData.LicenseeId, status);
            await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(gameProviderId, gameId, s.ExternalGameId, status, gameStatus);
        }
    }
}