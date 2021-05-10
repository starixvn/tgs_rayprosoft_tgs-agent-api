using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.AsiaGaming.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.AsiaGaming.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public sealed class AsiaGamingErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidStatusException), INVALID_SESSION},
                {typeof(BrandUnderMaintenanceException), INVALID_SESSION},
                {typeof(GameProviderUnderMaintenanceException), INVALID_SESSION},
                {typeof(GameUnderMaintenanceException), INVALID_SESSION},
                {typeof(MissingTokenException), INVALID_DATA},
                {typeof(PlayerDisabledException), INVALID_SESSION},
                {typeof(PlayerNotFoundException), INVALID_DATA},
                {typeof(PlayerLoginExpiredException), INVALID_SESSION},
                {typeof(PlayerHasInsufficientFundsException), INSUFFICIENT_FUNDS}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {Cancelled, OK},
                    {Settled, OK},
                    {Duplicate, OK},
                    {TransactionNotFound, INVALID_TRANSACTION},
                    {TransactionBeingCancelledNotFound, INVALID_TRANSACTION},
                    {TransactionBeingReferencedNotFound, INVALID_TRANSACTION},
                    {RoundNotFound, INVALID_TRANSACTION}
                };

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int)OK;
            }

            if (exception is InvalidGameActionException e &&
                ApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                return (int)error;
            }

            var type = exception.GetType();
            return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)ERROR;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}