using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.Saba.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.Saba.Enums.GameApiError;


namespace TGS.Apis.TgsApi.GameProviders.Saba.Services
{
    public sealed class SabaErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCredentialsException), InvalidToken},
                {typeof(MissingTokenException), InvalidToken},
                {typeof(PlayerLoginExpiredException), InvalidToken},
                {typeof(PlayerNotFoundException), AccountIsNotExist},
                {typeof(PlayerDisabledException), AccountIsLocked},
                {typeof(CountryRestrictedException), IpAddressRestricted},
                {typeof(CurrencyNotMatchingException), InvalidCurrency},
                {typeof(PlayerHasInsufficientFundsException), PlayerHasInsufficientFunds},
                {typeof(InvalidGameActionException), DatabaseError},
                {typeof(InvalidAmountException), InvalidAmount},
                {typeof(ArgumentException), IncorrectParameter},
                {typeof(ModelBindingException), IncorrectParameter},
                {typeof(FormatException), IncorrectParameter},
                {typeof(BrandUnderMaintenanceException), SystemIsUnderMaintenance},
                {typeof(GameProviderUnderMaintenanceException), SystemIsUnderMaintenance},
                {typeof(GameUnderMaintenanceException), SystemIsUnderMaintenance},
                {typeof(InvalidStatusException), SystemError},
                {typeof(InvalidOperationException), SystemError}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {Settled, Success},
                    {TransactionNotFound, NoTicket},
                    {TransactionBeingCancelledNotFound, NoTicket},
                    {TransactionBeingReferencedNotFound, NoTicket},
                    {RoundNotFound, NoTicket},
                    {GameNotFound, NoTicket}
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