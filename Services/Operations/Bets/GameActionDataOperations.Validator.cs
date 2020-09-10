using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Infrastructures.Extensions;
using TGS.Services.Operations.Models;
using TGS.Services.Operations.Models.Data;

namespace TGS.Services.Operations.Bets
{
    public sealed partial class GameActionDataOperations : IGameActionDataOperations
    {
        private void ValidateAmounts(GameActionState s)
        {
            s.ValidateAmount("Amount", s.Amount);

            if (s.Commission.HasValue)
            {
                _amountValidator.ThrowIfAmountIsNegative(nameof(s.Commission), s.Commission.Value);
            }
        }

        private void ThrowIfHaveNoRequiredIds(GameActionState s)
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

            if (s.GameActionType.IsNotAllOf(GameActionType.PlaceBet) && s.ExternalBetId.IsNullOrEmpty())
            {
                throw new InvalidGameActionException("Missing external bet ID.");
            }
        }

        private async Task ThrowIfExternalTransactionIdBelongsToDifferentRoundAsync(ITgsRepository repo, string externalRoundId, string externalTransactionId)
        {
            var foundExternalRoundId = await GetExternalRoundIdAsync(repo, externalTransactionId);
            if (foundExternalRoundId.IsNotNullOrEmpty() && foundExternalRoundId.IgnoreCaseNotEquals(externalTransactionId))
            {
                throw new InvalidGameActionException(
                       $"Expected the reference transaction '{externalTransactionId}' to belong to round '{externalRoundId}' but it belongs to '{foundExternalRoundId}'",
                       InvalidGameActionErrorCode.TransactionNotFound);
            }
        }

        private GameActionResult ValidateReferenceAction(GameActionState s)
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

        private void ThrowIfReferenceGameActionOrRoundIsNotFound(GameActionState s)
        {
            if (s.HasExtRefTransactionId && s.ReferencedGameAction == null)
            {
                if (s.GameActionType == GameActionType.CancelTransaction)
                {
                    throw new InvalidGameActionException(
                        $"Transaction being cancelled not found, external transaction ID: {s.ExternalTransactionReferenceId}",
                        InvalidGameActionErrorCode.TransactionBeingCancelledNotFound);
                }

                throw new InvalidGameActionException(
                    $"Transaction being referenced not found, transaction ID: {s.ExternalTransactionReferenceId} for game provider: {s.GameProviderCode}",
                    InvalidGameActionErrorCode.TransactionBeingReferencedNotFound);
            }

            if (s.MustHaveExistingRound && s.InternalRoundId == null)
            {
                throw new InvalidGameActionException(
                    $"Cannot find expected round with id={s.ExternalRoundId}",
                    InvalidGameActionErrorCode.RoundNotFound);
            }
        }
    }
}