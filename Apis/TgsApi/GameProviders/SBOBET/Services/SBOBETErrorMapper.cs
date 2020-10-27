// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public sealed class SBOBETErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(PlayerDisabledException), GameApiError.UserBlocked},
                {typeof(PlayerLoginExpiredException), GameApiError.PlayerNotLogin},
                {typeof(PlayerNotFoundException), GameApiError.PlayerNotFound},
                {typeof(ArgumentException), GameApiError.InvalidArguments},
                {typeof(FormatException), GameApiError.IncorrectFormat},
                {typeof(InvalidAmountException), GameApiError.InvalidAmount},
                {typeof(InvalidOperationException), GameApiError.SystemError},
                {typeof(InvalidStatusException), GameApiError.SystemError},
                {typeof(InvalidGameActionException), GameApiError.InvalidGameAction},
                {typeof(GameProviderUnderMaintenanceException), GameApiError.UnderMaintenance},
                {typeof(GameUnderMaintenanceException), GameApiError.UnderMaintenance},
                {typeof(LicenseeUnderMaintenanceException), GameApiError.UnderMaintenance},
                {typeof(InvalidCredentialsException), GameApiError.InvalidCredentials}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {Settled, GameApiError.BetAlreadySettled},
                    {TransactionNotFound, GameApiError.BetNotExists},
                    {TransactionBeingCancelledNotFound, GameApiError.BetNotExists},
                    {TransactionBeingReferencedNotFound, GameApiError.BetNotExists},
                    {RoundNotFound, GameApiError.BetNotExists}
                };

        int IErrorMapper.GetHttpStatusByException(Exception exception)
        {
            return StatusCodes.Status200OK;
        }

        string IErrorMapper.GetMessageByException(Exception exception)
        {
            return exception.Message;
        }

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int) GameApiError.Success;
            }

            if (exception is InvalidGameActionException e &&
                ApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                return (int) error;
            }

            var type = exception.GetType();
            if (!ApiErrorByException.TryGetValue(type, out error))
            {
                error = GameApiError.SystemError;
            }

            return (int) error;
        }
    }
}