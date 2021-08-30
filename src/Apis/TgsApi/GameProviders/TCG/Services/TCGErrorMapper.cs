using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.TCG.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.TCG.Enums.GameApiError;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    public sealed class TCGErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCredentialsException), InvalidSignature},
                {typeof(MissingTokenException), InvalidParametersFailedToDecryptTheParameters},
                {typeof(PlayerLoginExpiredException), UserDoesNotExists},
                {typeof(PlayerNotFoundException), UserDoesNotExists},
                {typeof(PlayerDisabledException), UserDoesNotExists},
                {typeof(PlayerHasInsufficientFundsException), InsufficientMerchantCreditToFundIn},
                {typeof(InvalidGameActionException), MethodNotFound},
                {typeof(InvalidAmountException), InsufficientMerchantCreditToFundIn},
                {typeof(ArgumentException), InvalidParametersFailedToDecryptTheParameters},
                {typeof(ModelBindingException), MissingRequiredParameter},
                {typeof(InvalidStatusException), UnknownSystemErrorPleaseContactTCGCustomerSupport},
                {typeof(InvalidOperationException), MethodNotFound}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {None, MethodNotFound},
                    { Duplicate, GameApiError.TransactionNotFound},
                    { Cancelled, GameApiError.TransactionNotFound},
                    { Settled, GameApiError.TransactionNotFound},
                    { InvalidGameActionErrorCode.TransactionNotFound,GameApiError. TransactionNotFound},
                    { TransactionBeingCancelledNotFound, GameApiError.TransactionNotFound},
                    { TransactionBeingReferencedNotFound, GameApiError.TransactionNotFound},
                    { RoundNotFound ,GameApiError.TransactionNotFound},
                    { GameNotFound, GameApiError.TransactionNotFound},
                    { ActionProhibited, GameApiError.TransactionNotFound}
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
            return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)UnknownSystemErrorPleaseContactTCGCustomerSupport;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}
