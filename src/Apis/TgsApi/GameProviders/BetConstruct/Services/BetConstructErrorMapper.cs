using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.BetConstruct.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.BetConstruct.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct.Services
{
    public sealed class BetConstructErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                //{typeof(InvalidCredentialsException), InvalidSignature},
                {typeof(MissingTokenException), PasswordOrAuthTokenError},
                {typeof(PlayerLoginExpiredException), UserIsDismissed},
                {typeof(PlayerNotFoundException), UserIsDismissed},
                {typeof(PlayerDisabledException), UserIsBlocked},
                //{typeof(PlayerHasInsufficientFundsException), InsufficientMerchantCreditToFundIn},
                //{typeof(InvalidGameActionException), MethodNotFound},
                //{typeof(InvalidAmountException), InsufficientMerchantCreditToFundIn},
                //{typeof(ArgumentException), InvalidParametersFailedToDecryptTheParameters},
                //{typeof(ModelBindingException), MissingRequiredParameter},
                //{typeof(InvalidStatusException), UnknownSystemErrorPleaseContactTCGCustomerSupport},
                //{typeof(InvalidOperationException), MethodNotFound}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {TransactionNotFound, InternalError}//TODO:需修改對應的InvalidGameActionErrorCode
                    //{TransactionBeingCancelledNotFound, BetSlipDoesNotExist},
                    //{TransactionBeingReferencedNotFound, BetSlipDoesNotExist},
                    //{RoundNotFound, BetSlipDoesNotExist},
                    //{GameNotFound, BetSlipDoesNotExist}
                };

        public int GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int)0;
            }

            if (exception is InvalidGameActionException e &&
                ApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
            {
                return (int)error;
            }

            var type = exception.GetType();
            return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)error;
        }

        public int GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;
        public string GetMessageByException(Exception exception) =>
            exception.Message;
    }
}
