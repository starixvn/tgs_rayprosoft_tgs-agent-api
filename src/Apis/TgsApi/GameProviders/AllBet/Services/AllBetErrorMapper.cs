using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.AllBet.Exceptions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.AllBet.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.AllBet.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public class AllBetErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> _ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidOperatorIdException), InvalidOperatorId},
                {typeof(InvalidCredentialsException), InvalidSignature},
                {typeof(MissingTokenException), LoginProhibited},
                {typeof(PlayerLoginExpiredException), LoginProhibited},
                {typeof(PlayerNotFoundException), UserIdNotFound},
                {typeof(PlayerDisabledException), LoginProhibited},
                {typeof(CountryRestrictedException), LoginProhibited},
                {typeof(CurrencyNotMatchingException), LoginProhibited},
                {typeof(PlayerHasInsufficientFundsException), InsufficientFunds},
                {typeof(InvalidGameActionException), InvalidGameActionOperation},
                {typeof(InvalidAmountException), InvalidArgument},
                {typeof(ArgumentException), InvalidArgument},
                {typeof(ModelBindingException), InvalidArgument},
                {typeof(FormatException), InvalidArgument},
                {typeof(BrandUnderMaintenanceException), UnderMaintenance},
                {typeof(GameProviderUnderMaintenanceException), UnderMaintenance},
                {typeof(GameUnderMaintenanceException), UnderMaintenance},
                {typeof(InvalidStatusException), SystemError},
                {typeof(InvalidOperationException), SystemError}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            _ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {TransactionNotFound, TransactionIsNotFound},
                    {TransactionBeingCancelledNotFound, TransactionIsNotFound},
                    {TransactionBeingReferencedNotFound, TransactionIsNotFound},
                    {RoundNotFound, TransactionIsNotFound}
                };

        public int GetApiErrorByException(Exception exception)
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

        public int GetHttpStatusByException(Exception exception)
            => StatusCodes.Status200OK;

        public string GetMessageByException(Exception exception)
            => exception.Message;
    }
}