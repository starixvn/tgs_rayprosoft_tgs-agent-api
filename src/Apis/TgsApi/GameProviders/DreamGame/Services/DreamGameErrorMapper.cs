using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Enums;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using static TGS.Apis.TgsApi.GameProviders.DreamGame.Enums.GameApiError;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Services;

public sealed class DreamGameErrorMapper : IErrorMapper
{
    private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
        new Dictionary<Type, GameApiError>
        {
            {typeof(InvalidCredentialsException), ApiKeyError},
            {typeof(MissingTokenException), TokenVerificationFailed},
            {typeof(PlayerLoginExpiredException), AccountIsLoggedOut},
            {typeof(PlayerNotFoundException), AccountDoesNotExist},
            {typeof(PlayerDisabledException), AccountsAreLocked},
            {typeof(PlayerHasInsufficientFundsException), InsufficientBalance},
            {typeof(InvalidGameActionException), OperationFailed},
            {typeof(InvalidAmountException), DataFormatError},
            {typeof(ArgumentException), ParameterError},
            {typeof(ModelBindingException), ParameterError},
            {typeof(InvalidStatusException), OperationFailed},
            {typeof(InvalidOperationException), IllegalOperation}
        };

    private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
        ApiErrorByInvalidGameActionErrorCode =
            new Dictionary<InvalidGameActionErrorCode, GameApiError>
            {
                {TransactionNotFound, TransferFailed},
                {TransactionBeingCancelledNotFound, TransferFailed},
                {TransactionBeingReferencedNotFound, TransferFailed},
                {RoundNotFound, TransferFailed},
                {GameNotFound, TransferFailed}
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
        return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)UnknownError;
    }

    int IErrorMapper.GetHttpStatusByException(Exception exception) =>
        StatusCodes.Status200OK;

    string IErrorMapper.GetMessageByException(Exception exception) =>
        exception.Message;
}
