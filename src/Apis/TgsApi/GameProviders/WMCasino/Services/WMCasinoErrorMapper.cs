using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.WMCasino.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.WMCasino.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public class WMCasinoErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCredentialsException), InvalidCredential},
                {typeof(MissingTokenException), InvalidParameter},
                {typeof(PlayerLoginExpiredException), InvalidPlayerCode},
                {typeof(PlayerNotFoundException), InvalidPlayerCode},
                {typeof(PlayerDisabledException), SuspendPlayer},
                {typeof(CountryRestrictedException), SystemError},
                {typeof(PlayerHasInsufficientFundsException), InsufficientFunds},
                {typeof(InvalidGameActionException), SystemError},
                {typeof(InvalidAmountException), InvalidParameter},
                {typeof(ArgumentException), InvalidParameter},
                {typeof(ModelBindingException), InvalidParameter},
                {typeof(FormatException), InvalidParameter},
                {typeof(BrandUnderMaintenanceException), Maintenance},
                {typeof(GameProviderUnderMaintenanceException), Maintenance},
                {typeof(GameUnderMaintenanceException), Maintenance},
                {typeof(InvalidStatusException), InvalidParameter},
                {typeof(InvalidOperationException), SystemError}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {Settled, Success},
                    {TransactionNotFound, TransactionIsNotFound},
                    {TransactionBeingCancelledNotFound, TransactionIsNotFound},
                    {TransactionBeingReferencedNotFound, TransactionIsNotFound},
                    {RoundNotFound, RoundIsNotFound},
                    {GameNotFound, GameIsNotFound}
                };

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
            return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)SystemError;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}