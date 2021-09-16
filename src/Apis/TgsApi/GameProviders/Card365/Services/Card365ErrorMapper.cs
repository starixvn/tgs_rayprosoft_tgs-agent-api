using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.Card365.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.Card365.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public class Card365ErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> _ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                { typeof(InvalidCredentialsException), NotAuthorized },
                { typeof(MissingTokenException), InvalidParameters },
                { typeof(PlayerLoginExpiredException), NotAuthorized },
                { typeof(PlayerNotFoundException), InvalidUsername },
                { typeof(PlayerDisabledException), AccountBlocked },
                { typeof(CountryRestrictedException), SystemError },
                { typeof(PlayerHasInsufficientFundsException), InsufficientFunds },
                { typeof(InvalidGameActionException), SystemError },
                { typeof(InvalidAmountException), InvalidAmount },
                { typeof(ArgumentException), InvalidParameters },
                { typeof(ModelBindingException), InvalidParameters },
                { typeof(FormatException), InvalidParameters },
                { typeof(BrandUnderMaintenanceException), UnderMaintenance },
                { typeof(GameProviderUnderMaintenanceException), UnderMaintenance },
                { typeof(GameUnderMaintenanceException), UnderMaintenance },
                { typeof(InvalidStatusException), InvalidParameters },
                { typeof(InvalidOperationException), SystemError }
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            _ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    { Settled, Success },
                    { Cancelled, Success },
                    { Duplicate, Success },
                    { TransactionNotFound, NoData },
                    { TransactionBeingCancelledNotFound, NoData },
                    { TransactionBeingReferencedNotFound, NoData },
                    { RoundNotFound, NoData },
                    { GameNotFound, GameIdNotExist }
                };

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int)Success;
            }

            if (exception is InvalidGameActionException e &&
                _ApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                return (int)error;
            }

            var type = exception.GetType();
            return _ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)SystemError;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}