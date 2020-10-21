using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;

namespace TGS.Core.Infrastructures.ErrorMappers
{
    public sealed class TgsErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, TgsApiError> TgsApiErrorByException =
            new Dictionary<Type, TgsApiError>
            {
                { typeof(PlayerDisabledException), TgsApiError.UserBlocked },
                { typeof(PlayerNotFoundException), TgsApiError.PlayerNotFound },
                { typeof(ArgumentException), TgsApiError.InvalidArguments },
                { typeof(FormatException), TgsApiError.IncorrectFormat },
                { typeof(InvalidAmountException), TgsApiError.InvalidAmount },
                { typeof(InvalidOperationException), TgsApiError.SystemError },
                { typeof(InvalidStatusException), TgsApiError.SystemError },
                { typeof(InvalidGameActionException), TgsApiError.InvalidGameAction },
                { typeof(GameProviderUnderMaintenanceException), TgsApiError.UnderMaintenance },
                { typeof(GameUnderMaintenanceException), TgsApiError.UnderMaintenance },
                { typeof(InvalidCredentialsException), TgsApiError.InvalidCredentials },
                { typeof(ClientIdNotFoundException), TgsApiError.ClientIdNotFound },
                { typeof(InvalidCaptchaException), TgsApiError.InvalidCaptcha },
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, TgsApiError> TgsApiErrorByInvalidGameActionErrorCode =
            new Dictionary<InvalidGameActionErrorCode, TgsApiError>
            {
                { Duplicate, TgsApiError.DuplicateTransaction },
                { TransactionNotFound, TgsApiError.TransactionIdNotFound },
                { TransactionBeingCancelledNotFound, TgsApiError.TransactionIdNotFound },
                { TransactionBeingReferencedNotFound, TgsApiError.TransactionIdNotFound }
            };

        private static readonly IReadOnlyDictionary<TgsApiError, int> HttpCodeByException =
           new Dictionary<TgsApiError, int>
           {
               { TgsApiError.InvalidArguments, StatusCodes.Status500InternalServerError},
               { TgsApiError.ClientIdNotFound, StatusCodes.Status401Unauthorized },
               { TgsApiError.InvalidCredentials, StatusCodes.Status401Unauthorized},
               { TgsApiError.InvalidCaptcha, StatusCodes.Status401Unauthorized},
               { TgsApiError.PlayerNotFound, StatusCodes.Status401Unauthorized },
               { TgsApiError.TransactionIdNotFound, StatusCodes.Status500InternalServerError },
               { TgsApiError.DuplicateTransaction, StatusCodes.Status400BadRequest },
               { TgsApiError.CurrencyMismatch, StatusCodes.Status500InternalServerError },
               { TgsApiError.SystemError, StatusCodes.Status500InternalServerError },
               { TgsApiError.UnderMaintenance, StatusCodes.Status503ServiceUnavailable },
               { TgsApiError.OperationNotAllowed, StatusCodes.Status405MethodNotAllowed },
               { TgsApiError.CountryRestricted, StatusCodes.Status403Forbidden },
               { TgsApiError.TestPlayerStakeAmountCapExceeded, StatusCodes.Status500InternalServerError },
           };

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int)TgsApiError.None;
            }

            if (exception is InvalidGameActionException e &&
                TgsApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out TgsApiError error))
            {
                return (int)error;
            }

            var type = exception.GetType();
            if (!TgsApiErrorByException.TryGetValue(type, out error))
            {
                error = TgsApiError.SystemError;
            }

            return (int)error;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception)
        {
            if (exception == null)
            {
                return StatusCodes.Status200OK;
            }

            if (exception is not InvalidGameActionException e ||
                !TgsApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                var type = exception.GetType();
                if (!TgsApiErrorByException.TryGetValue(type, out error))
                {
                    return StatusCodes.Status500InternalServerError;
                }
            }

            return HttpCodeByException.TryGetValue(error, out var statusCode) ?
                statusCode :
                StatusCodes.Status500InternalServerError;
        }

        string IErrorMapper.GetMessageByException(Exception exception)
        {
            return exception.Message;
        }
    }
}