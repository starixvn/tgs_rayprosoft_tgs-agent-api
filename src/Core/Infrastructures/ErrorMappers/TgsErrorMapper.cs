using System;
using System.Collections.Generic;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Core.Models.Enums.TgsApiError;

namespace TGS.Core.Infrastructures.ErrorMappers
{
    public sealed class TgsErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, TgsApiError> TgsApiErrorByException =
            new Dictionary<Type, TgsApiError>
            {
                {typeof(InvalidCredentialsException), InvalidCredentials},
                {typeof(MissingTokenException), InvalidOrExpiredPlayerToken},
                {typeof(PlayerLoginExpiredException), InvalidOrExpiredPlayerToken},
                {typeof(InvalidCaptchaException), InvalidCaptcha},
                {typeof(ClientIdNotFoundException), ClientIdNotFound},
                {typeof(PlayerNotFoundException), PlayerNotFound},
                {typeof(PlayerDisabledException), UserBlocked},
                {typeof(CountryRestrictedException), CountryRestricted},
                {typeof(CurrencyNotMatchingException), CurrencyMismatch},
                {typeof(PlayerHasInsufficientFundsException), InsufficientFunds},
                {typeof(InvalidInvitationCodeException), InvalidInvitationCode},
                {typeof(InvalidGameActionException), InvalidGameAction},
                {typeof(InvalidAmountException), InvalidAmount},
                {typeof(ArgumentException), InvalidArguments},
                {typeof(ModelBindingException), InvalidArguments},
                {typeof(FormatException), IncorrectFormat},
                {typeof(BrandUnderMaintenanceException), UnderMaintenance},
                {typeof(GameProviderUnderMaintenanceException), UnderMaintenance},
                {typeof(GameUnderMaintenanceException), UnderMaintenance},
                {typeof(InvalidStatusException), InvalidStatus},
                {typeof(InvalidOperationException), SystemError},
                {typeof(OperationNotAllowedException), OperationNotAllowed},
                {typeof(GameProviderApiErrorException), ProviderApiError}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, TgsApiError>
            TgsApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, TgsApiError>
                {
                    {Duplicate, DuplicateTransaction},
                    {TransactionNotFound, TransactionIdNotFound},
                    {TransactionBeingCancelledNotFound, TransactionIdNotFound},
                    {TransactionBeingReferencedNotFound, TransactionIdNotFound},
                    {RoundNotFound, RoundIdNotFound}
                };

        private static readonly IReadOnlyDictionary<TgsApiError, int> HttpCodeByException =
            new Dictionary<TgsApiError, int>
            {
                {InvalidCredentials, Status401Unauthorized},
                {InvalidOrExpiredPlayerToken, Status401Unauthorized},
                {InvalidCaptcha, Status401Unauthorized},
                {ClientIdNotFound, Status401Unauthorized},
                {PlayerNotFound, Status401Unauthorized},
                {UserBlocked, Status403Forbidden},
                {CountryRestricted, Status403Forbidden},
                {CurrencyMismatch, Status500InternalServerError},
                {InsufficientFunds, Status500InternalServerError},
                {InvalidInvitationCode, Status500InternalServerError},
                {InvalidGameAction, Status500InternalServerError},
                {TransactionIdNotFound, Status500InternalServerError},
                {DuplicateTransaction, Status500InternalServerError},
                {InvalidAmount, Status500InternalServerError},
                {InvalidArguments, Status400BadRequest},
                {IncorrectFormat, Status400BadRequest},
                {UnderMaintenance, Status500InternalServerError},
                {InvalidStatus, Status500InternalServerError},
                {SystemError, Status500InternalServerError},
                {OperationNotAllowed, Status405MethodNotAllowed},
                {ProviderApiError, Status500InternalServerError}
            };

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int)TgsApiError.None;
            }

            if (exception is InvalidGameActionException e &&
                TgsApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                return (int)error;
            }

            var type = exception.GetType();
            return TgsApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)SystemError;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception)
        {
            if (exception == null)
            {
                return Status200OK;
            }

            int statusCode;
            if (exception is InvalidGameActionException e &&
                TgsApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                return HttpCodeByException.TryGetValue(error, out statusCode)
                    ? statusCode
                    : Status500InternalServerError;
            }

            var type = exception.GetType();
            return TgsApiErrorByException.TryGetValue(type, out error) &&
                HttpCodeByException.TryGetValue(error, out statusCode)
                    ? statusCode
                    : Status500InternalServerError;
        }

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}