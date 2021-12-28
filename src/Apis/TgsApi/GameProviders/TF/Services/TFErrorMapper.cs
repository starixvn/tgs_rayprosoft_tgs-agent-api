using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.TF.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.TF.Enums.GameApiError;


namespace TGS.Apis.TgsApi.GameProviders.TF.Services
{
    public sealed class TFErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
           new Dictionary<Type, GameApiError>
           {
                {typeof(InvalidCredentialsException), InvalidAuthenticationCredential},
                //{typeof(MissingTokenException), PasswordOrAuthTokenError},
                //{typeof(PlayerLoginExpiredException), UserIsDismissed},
                //{typeof(PlayerNotFoundException), UserIsDismissed},
                {typeof(PlayerDisabledException), AccountIsFrozen},
                {typeof(PlayerHasInsufficientFundsException), TryingToWithdrawMoreThanCurrentBalance},
                {typeof(InvalidGameActionException), Undefined},
                //{typeof(InvalidAmountException), InternalError},
                {typeof(ArgumentException), MissingRequiredFields},
                {typeof(ModelBindingException), Undefined},
                //{typeof(InvalidStatusException), InternalError},
                {typeof(InvalidOperationException), Undefined}
           };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    //{TransactionNotFound, InternalError},
                    //{TransactionBeingCancelledNotFound, InternalError},
                    //{TransactionBeingReferencedNotFound, InternalError},
                    //{RoundNotFound, InternalError},
                    //{GameNotFound, InternalError}
                };

        public int GetApiErrorByException(Exception exception)
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
            return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)error;
        }

        public int GetHttpStatusByException(Exception exception)
        {
            if (exception == null)
            {
                return StatusCodes.Status200OK;
            }
            else
            {
                return StatusCodes.Status400BadRequest;
            }
        }

        public string GetMessageByException(Exception exception) =>
            exception.Message;
    }
}
