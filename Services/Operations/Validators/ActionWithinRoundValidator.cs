using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Services.Operations.Models.Data;
using static TGS.Core.Models.Enums.GameActionType;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;

namespace TGS.Services.Operations.Validators
{
    public interface IActionWithinRoundValidator
    {
        Exception FindError(BetActionData current, IList<BetActionData> previousActions);
    }

    public sealed class ActionWithinRoundValidator : IActionWithinRoundValidator
    {
        Exception IActionWithinRoundValidator.FindError(BetActionData current, IList<BetActionData> previousActions)
        {
            if (IsClosingRoundOnPlaceBet(current, out var error))
                return error;

            if (IsFoundUnallowedReferencedActionType(current, out error))
                return error;

            if (IsCurrentReferenceCancelled(current, previousActions, out error))
                return error;

            if (IsCancellingActionThatHasUncencelledReferences(current, previousActions, out error))
                return error;

            var previouslyClosed = FindPreviouslyClosed(previousActions);

            if (IsExplicitlyClosingAlreadyClosedRound(current, previouslyClosed, out error))
                return error;

            if (IsNotExplicitlyOpeningAlreadyClosedRound(current, previouslyClosed, out error))
                return error;

            // no irregularities found
            return null;
        }

        private static bool IsClosingRoundOnPlaceBet(BetActionData current, out Exception error)
        {
            if (current.IsClosingRound && current.Type == PlaceBet)
            {
                error = new InvalidGameActionException(
                            $"Cannot close round on place bet action with ID='{current.ExternalId}' " +
                            $"for round with ID='{current.ExternalRoundId}'",
                            ActionProhibited);
                return true;
            }

            error = null;
            return false;
        }

        // ensured rules described in the following table (detects NO cases there): /Readme/BusinessLogic/ReferencedActionsRules.md
        private static bool IsFoundUnallowedReferencedActionType(BetActionData current, out Exception error)
        {
            error = null;
            if (current.Reference == null)
            {
                return false;
            }

            var gameActionType = current.Type;
            var refId = current.Reference.Id;
            var refType = current.Reference.Type;

            // no action is allowed with reference to CANCEL action
            // rows:Cancel
            // cols:ANY
            if (refType == CancelTransaction)
            {
                error = new InvalidGameActionException(
                        $"You cannot perform any action on action of type '{refType}' for reference with ID='{refId}'",
                        ActionProhibited);
                return true;
            }

            // rows:Lock
            // cols:Free,Win,Lose,Tie
            if (refType == LockAmount && gameActionType.IsOneOf(FreeBet, WinBet, LoseBet, TieBet))
            {
                error = new InvalidGameActionException(
                    $"It is not allowed to have '{gameActionType}' action with a referenced action of type '{refType}' for reference with ID='{refId}'",
                    ActionProhibited);
                return true;
            }

            // rows:Unlock
            // cols:Lock,Unlock
            if (refType == UnlockAmount && gameActionType.IsOneOf(LockAmount, UnlockAmount))
            {
                error = new InvalidGameActionException(
                    $"It is not allowed to have '{gameActionType}' action with a referenced action of type '{refType}' for reference with ID='{refId}'",
                    ActionProhibited);
                return true;
            }

            // rows:Free,Win,Lose,Tie
            // cols:Lock,Unlock,Place,Win,Lose,Tie
            if (gameActionType.IsOneOf(LockAmount, UnlockAmount, PlaceBet, WinBet, LoseBet, TieBet) &&
                refType.IsOneOf(FreeBet, WinBet, LoseBet, TieBet))
            {
                error = new InvalidGameActionException(
                    $"It is not allowed to have '{gameActionType}' action with a referenced action of type '{refType}' for reference with ID='{refId}'",
                    ActionProhibited);
                return true;
            }

            return false;
        }

        private static bool IsCurrentReferenceCancelled(BetActionData current, IList<BetActionData> previousActions, out Exception error)
        {
            error = null;
            // current action cannot be referencing another action that has been cancelled
            var cancellationOfReference = current.ReferenceId == null
                    ? null
                    : previousActions.FirstOrDefault(a => a.Type == CancelTransaction && a.ReferenceId == current.ReferenceId);

            if (cancellationOfReference != null)
            {
                error = new InvalidGameActionException(
                    "Cannot reference action that has been cancelled. " +
                    $"The current action with ID='{current.ExternalId}' " +
                    $"for round with ID='{current.ExternalRoundId}' " +
                    $"has a reference to action of type '{current.Reference.Type}' with ID='{current.Reference.ExternalId}' " +
                    $"that has been cancelled by action with ID='{cancellationOfReference.ExternalId}'",
                    Cancelled);
            }

            return error != null;
        }

        private static bool IsCancellingActionThatHasUncencelledReferences(BetActionData current, IList<BetActionData> previousActions, out Exception error)
        {
            error = null;

            // if we cancel PlaceBet or LockAmount
            if (current.Type == CancelTransaction && current.Reference.Type.IsOneOf(PlaceBet, LockAmount))
            {
                var referencesOfTheReference = previousActions
                    .Where(ga => ga.Reference != null && ga.ReferenceId == current.ReferenceId)
                    .ToArray();

                var existingReferenceIds = referencesOfTheReference
                    .Select(e => (Guid?)e.Id)
                    .ToList();

                var cancelledReferences = previousActions
                    .Where(ga => existingReferenceIds.Contains(ga.ReferenceId) && ga.Type == CancelTransaction)
                    .Select(ga => ga.ReferenceId)
                    .ToHashSet();

                // if some of the existing references of current action does not have cancelation:
                // (if we have any referencesOfTheReference[...].Id not in cancelledReferences[...].Id)
                // return error, because all references must be canceled, to cancel PlaceBet
                var uncancelledReferences = referencesOfTheReference.FirstOrDefault(curr => !cancelledReferences.Contains(curr.Id));

                if (uncancelledReferences != null)
                {
                    var refId = current.Reference.Id;
                    error = new InvalidGameActionException(
                            $"Cannot cancel {current.Reference.Type} action with ID='{refId}' " +
                            $"that is referenced by uncancelled action with ID='{uncancelledReferences.Id}' and Type='{uncancelledReferences.Type}'",
                            Settled);
                }
            }

            return error != null;
        }

        private static BetActionData FindPreviouslyClosed(IList<BetActionData> actions)
        {
            if (actions == null)
                return null;

            BetActionData closingAction = null;
            foreach (var a in actions)
            {
                if (a.IsClosingRound)
                {
                    closingAction = a;
                }
                else if (closingAction != null && a.IsExplicitlyNotClosingRound) // re-open
                {
                    closingAction = null;
                }
            }

            return closingAction;
        }

        private static bool IsExplicitlyClosingAlreadyClosedRound(BetActionData current, BetActionData previouslyClosed, out Exception error)
        {
            error = null;
            if (previouslyClosed == null)
            {
                return false;
            }

            // no explicit close if previously closed
            if (current.IsExplicitlyClosingRound)
            {
                error = new InvalidGameActionException(
                    "Cannot explicitly close already closed round with 'roundclosed=true' " +
                    $"Attempt to perform close action with ID='{current.ExternalId}' " +
                    $"but within the same round with ID='{current.ExternalRoundId}' we already have " +
                    $"another closing action with ID='{previouslyClosed.ExternalId}'",
                    current.ExternalId == previouslyClosed.ExternalId ? Duplicate : ActionProhibited);

                return true;
            }

            return false;
        }

        private static bool IsNotExplicitlyOpeningAlreadyClosedRound(BetActionData current, BetActionData previouslyClosed, out Exception error)
        {
            error = null;
            if (previouslyClosed == null)
            {
                return false;
            }

            // roundclosed=null or missing 'roundclosed' field
            if (!current.IsExplicitlyClosingRound && !current.IsExplicitlyNotClosingRound)
            {
                error = new InvalidGameActionException(
                    "Cannot perform action on already closed round, given that you are not reopening it 'roundclosed=null or missing' " +
                    $"Attempt to perform action with ID='{current.ExternalId}' " +
                    $"but within the same round with ID='{current.ExternalRoundId}' we already have " +
                    $"another closing action with ID='{previouslyClosed.ExternalId}' and your action is not reopening the round with roundclosed=false",
                    ActionProhibited);
                return true;
            }

            return false;
        }
    }
}