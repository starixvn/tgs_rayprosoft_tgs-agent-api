using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.JDB.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.JDB.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public sealed class JDBErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(BrandUnderMaintenanceException), SystemIsMaintained},
                {typeof(GameProviderUnderMaintenanceException), SystemIsMaintained},
                {typeof(GameUnderMaintenanceException), SystemIsMaintained},
                {typeof(PlayerDisabledException), UserIdCannotBeFound},
                {typeof(PlayerNotFoundException), UserIdCannotBeFound},
                {typeof(PlayerLoginExpiredException), UserIdCannotBeFound},
                {typeof(PlayerHasInsufficientFundsException), BalanceNotEnough}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {Cancelled, Succeed},
                    {Settled, Succeed},
                    {Duplicate, DuplicateTransactions},
                    {TransactionNotFound, DataDoesNotExist},
                    {TransactionBeingCancelledNotFound, DataDoesNotExist},
                    {TransactionBeingReferencedNotFound, DataDoesNotExist},
                    {RoundNotFound, DataDoesNotExist}
                };

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int)Succeed;
            }

            if (exception is InvalidGameActionException e &&
                ApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                return (int)error;
            }

            var type = exception.GetType();
            return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)Failed;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}