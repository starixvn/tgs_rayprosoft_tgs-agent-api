using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.Skywind.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.Skywind.Enums.GameApiError;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public sealed class SkywindErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCredentialsException), GameTokenExpired},
                {typeof(MissingTokenException), GameTokenExpired},
                {typeof(PlayerLoginExpiredException), PlayerNotFound},
                {typeof(PlayerNotFoundException), PlayerNotFound},
                {typeof(PlayerDisabledException), PlayerIsPermanentlyBlocked},
                {typeof(PlayerHasInsufficientFundsException), InsufficientBalance},
                {typeof(InvalidGameActionException), MerchantInternalError},
                {typeof(InvalidAmountException), MerchantInternalError},
                {typeof(ArgumentException), MerchantInternalError},
                {typeof(ModelBindingException), MerchantInternalError},
                {typeof(InvalidStatusException), MerchantInternalError},
                {typeof(InvalidOperationException), MerchantInternalError}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {None, MerchantInternalError},
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
            return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)MerchantInternalError;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}
