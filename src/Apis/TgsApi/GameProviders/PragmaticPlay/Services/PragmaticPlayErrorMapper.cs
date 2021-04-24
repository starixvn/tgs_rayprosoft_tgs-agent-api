using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.PragmaticPlay.Enums;
using static TGS.Shared.Game.Models.PragmaticPlay.Enums.GameApiError;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public sealed class PragmaticPlayErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCredentialsException), InvalidHash},
                {typeof(InvalidStatusException), PlayerTokenInvalid},
                {typeof(BrandUnderMaintenanceException), GameIsNotFoundOrDisabled},
                {typeof(GameProviderUnderMaintenanceException), GameIsNotFoundOrDisabled},
                {typeof(GameUnderMaintenanceException), GameIsNotFoundOrDisabled},
                {typeof(MissingTokenException), PlayerTokenInvalid},
                {typeof(PlayerDisabledException), PlayerIsFrozen},
                {typeof(PlayerNotFoundException), PlayerTokenInvalid},
                {typeof(PlayerLoginExpiredException), PlayerTokenInvalid},
                {typeof(PlayerHasInsufficientFundsException), InsufficientBalance},
                {typeof(ArgumentException), BadParameters},
                {typeof(ArgumentNullException), BadParameters},
                {typeof(InvalidAmountException), BadParameters},
                {typeof(InvalidGameActionException), BetIsNotAllowed},
                {typeof(InvalidOperationException), InternalServerErrorWillNotReconcile}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {Cancelled, Success},
                    {Settled, Success},
                    {Duplicate, Success},
                    {TransactionNotFound, InternalServerErrorWillNotReconcile},
                    {TransactionBeingCancelledNotFound, InternalServerErrorWillNotReconcile},
                    {TransactionBeingReferencedNotFound, InternalServerErrorWillNotReconcile},
                    {RoundNotFound, InternalServerErrorWillNotReconcile}
                };

        private static readonly IReadOnlyDictionary<Type, int> HttpStatusByException =
            new Dictionary<Type, int>
            {
                {typeof(InvalidCredentialsException), Status403Forbidden},
                {typeof(InvalidStatusException), Status400BadRequest},
                {typeof(BrandUnderMaintenanceException), Status400BadRequest},
                {typeof(GameProviderUnderMaintenanceException), Status400BadRequest},
                {typeof(GameUnderMaintenanceException), Status400BadRequest},
                {typeof(MissingTokenException), Status403Forbidden},
                {typeof(PlayerDisabledException), Status403Forbidden},
                {typeof(PlayerNotFoundException), Status403Forbidden},
                {typeof(PlayerLoginExpiredException), Status403Forbidden},
                {typeof(PlayerHasInsufficientFundsException), Status500InternalServerError},
                {typeof(ArgumentException), Status400BadRequest},
                {typeof(ArgumentNullException), Status400BadRequest},
                {typeof(InvalidAmountException), Status400BadRequest},
                {typeof(InvalidGameActionException), Status500InternalServerError},
                {typeof(InvalidOperationException), Status500InternalServerError}
            };

        int IErrorMapper.GetHttpStatusByException(Exception exception)
        {
            if (exception == null)
            {
                return Status200OK;
            }

            if (exception is InvalidGameActionException e &&
                ApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error) && error == Success)
            {
                return Status200OK;
            }

            var type = exception.GetType();
            return HttpStatusByException.TryGetValue(type, out var status) ? status : Status400BadRequest;
        }

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int)Success;
            }

            if (exception is InvalidGameActionException e &&
                ApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                return (int)error;
            }

            var type = exception.GetType();
            return ApiErrorByException.TryGetValue(type, out error)
                ? (int)error
                : (int)InternalServerErrorWillNotReconcile;
        }
    }
}