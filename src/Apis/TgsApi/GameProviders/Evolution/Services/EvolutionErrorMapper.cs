using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.Evolution.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.Evolution.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public sealed class EvolutionErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCaptchaException), InvalidTokenId},
                {typeof(InvalidCredentialsException), InvalidTokenId},
                {typeof(MissingTokenException), InvalidTokenId},
                {typeof(PlayerLoginExpiredException), InvalidTokenId},
                {typeof(PlayerNotFoundException), InvalidTokenId},
                {typeof(PlayerDisabledException), AccountLocked},
                {typeof(PlayerHasInsufficientFundsException), InsufficientFunds},
                {typeof(InvalidGameActionException), FinalErrorActionFailed},
                {typeof(InvalidAmountException), InsufficientFunds},
                {typeof(ArgumentException), InvalidParameter},
                {typeof(ModelBindingException), UnknownError},
                {typeof(InvalidStatusException), UnknownError},
                {typeof(InvalidOperationException), FinalErrorActionFailed}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {None, UnknownError},
                    { Duplicate, SessionAlreadyClosed},
                    { Cancelled, SessionAlreadyClosed},
                    { Settled, SessionAlreadyClosed},
                    { InvalidGameActionErrorCode.TransactionNotFound,GameApiError. SessionAlreadyClosed},
                    { TransactionBeingCancelledNotFound, SessionAlreadyClosed},
                    { TransactionBeingReferencedNotFound, SessionAlreadyClosed},
                    { RoundNotFound ,GameApiError.SessionAlreadyClosed},
                    { GameNotFound, SessionAlreadyClosed},
                    { ActionProhibited, SessionAlreadyClosed}
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
}
