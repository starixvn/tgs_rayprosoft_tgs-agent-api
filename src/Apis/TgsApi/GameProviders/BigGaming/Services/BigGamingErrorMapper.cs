using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.BigGaming.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.BigGaming.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public sealed class BigGamingErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCredentialsException), AuthenticationSignIsInvalid},
                {typeof(MissingTokenException), AuthenticationSignIsInvalid},
                {typeof(PlayerLoginExpiredException), AuthenticationSignIsInvalid},
                {typeof(PlayerNotFoundException), AccountDoesNotExist},
                {typeof(PlayerDisabledException), CurrentUserHasBeenBannedAndLoginFailed},
                {typeof(PlayerHasInsufficientFundsException), InsufficientBettingBalance},
                {typeof(InvalidGameActionException), UnknownDataAccessException},
                {typeof(InvalidAmountException), AbnormalFlowOfBulkFundsIntoTheWarehouse},
                {typeof(ArgumentException), EncryptionAndDecryptionDataIsAbnormal},
                {typeof(ModelBindingException), ParametersPassedInAreIncorrect},
                {typeof(InvalidStatusException), UnknownException},
                {typeof(InvalidOperationException), UnknownException}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {TransactionNotFound, BetSlipDoesNotExist},
                    {TransactionBeingCancelledNotFound, BetSlipDoesNotExist},
                    {TransactionBeingReferencedNotFound, BetSlipDoesNotExist},
                    {RoundNotFound, BetSlipDoesNotExist},
                    {GameNotFound, BetSlipDoesNotExist}
                };

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int)0;
            }

            if (exception is InvalidGameActionException e &&
                ApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                return (int)error;
            }

            var type = exception.GetType();
            return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)UnknownException;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}
