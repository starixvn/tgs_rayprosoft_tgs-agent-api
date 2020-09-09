using System;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Infrastructures.Extensions;
using TGS.Services.Operations.Models;

namespace TGS.Services.Operations
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
                throw new InvalidGameActionException("Missing External Transaction Reference Id");
            }
            
            if (s.MustHaveExistingRound && !s.HasExtRoundId) 
            { 
                 throw new InvalidGameActionException("Missing External Round Id");
            }

            if (!s.HasExtGameId)
            { 
                throw new InvalidGameActionException(
                    $"Missing game external id for '{s.PlatformType}' of provider config with id={s.GameProviderConfigurationId}",
                    InvalidGameActionErrorCode.GameNotFound);
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

    }
}