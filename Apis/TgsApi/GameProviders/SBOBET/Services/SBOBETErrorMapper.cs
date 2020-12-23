// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using static TGS.Apis.TgsApi.GameModels.SBOBET.Enums.GameApiError;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public sealed class SBOBETErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCredentialsException), InvalidCredentials},
                {typeof(MissingTokenException), PlayerNotLogin},
                {typeof(PlayerLoginExpiredException), PlayerNotLogin},

                {typeof(PlayerNotFoundException), PlayerNotFound},
                {typeof(PlayerDisabledException), UserBlocked},
                {typeof(CountryRestrictedException), CountryRestricted},
                {typeof(CurrencyNotMatchingException), CurrencyMismatch},
                {typeof(PlayerHasInsufficientFundsException), InsufficientFunds},

                {typeof(InvalidGameActionException), InvalidGameAction},
                {typeof(InvalidAmountException), InvalidAmount},

                {typeof(ArgumentException), InvalidArguments},
                {typeof(ModelBindingException), InvalidArguments},
                {typeof(FormatException), IncorrectFormat},
                {typeof(BrandUnderMaintenanceException), UnderMaintenance},
                {typeof(GameProviderUnderMaintenanceException), UnderMaintenance},
                {typeof(GameUnderMaintenanceException), UnderMaintenance},
                {typeof(InvalidStatusException), SystemError},

                {typeof(InvalidOperationException), SystemError}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {Settled, BetAlreadySettled},
                    {TransactionNotFound, BetNotExists},
                    {TransactionBeingCancelledNotFound, BetNotExists},
                    {TransactionBeingReferencedNotFound, BetNotExists},
                    {RoundNotFound, BetNotExists}
                };

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int) Success;
            }

            if (exception is InvalidGameActionException e &&
                ApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                return (int) error;
            }

            var type = exception.GetType();
            return ApiErrorByException.TryGetValue(type, out error) ? (int) error : (int) SystemError;
        }
    }
}