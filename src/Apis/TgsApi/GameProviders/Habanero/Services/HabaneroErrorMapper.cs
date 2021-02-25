using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.Habanero.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.Habanero.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Services
{
    public sealed class HabaneroErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCredentialsException), AuthorizationFailed},
                {typeof(InvalidStatusException), AuthorizationFailed},
                {typeof(BrandUnderMaintenanceException), AuthorizationFailed},
                {typeof(GameProviderUnderMaintenanceException), AuthorizationFailed},
                {typeof(GameUnderMaintenanceException), AuthorizationFailed},
                {typeof(MissingTokenException), AuthorizationFailed},
                {typeof(PlayerDisabledException), AuthorizationFailed},
                {typeof(PlayerNotFoundException), AuthorizationFailed},
                {typeof(PlayerLoginExpiredException), AuthorizationFailed},
                {typeof(PlayerHasInsufficientFundsException), InsufficientFunds}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {Cancelled, Success},
                    {Settled, Success}, 
                    {Duplicate, DuplicateTransaction},
                    {TransactionNotFound, TransactionNotExist},
                    {TransactionBeingCancelledNotFound, TransactionNotExist},
                    {TransactionBeingReferencedNotFound, TransactionNotExist},
                    {RoundNotFound, TransactionNotExist}
                };

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
            return ApiErrorByException.TryGetValue(type, out error) ? (int) error : (int) Uncategorized;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}