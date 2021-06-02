using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.SexyGaming.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.SexyGaming.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public sealed class SexyGamingErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCredentialsException), InvalidToken},
                {typeof(MissingTokenException), InvalidCertificate},
                {typeof(PlayerLoginExpiredException), InvalidUserId},
                {typeof(PlayerNotFoundException), InvalidUserId},
                {typeof(PlayerDisabledException), AccountIsSuspend},
                {typeof(PlayerHasInsufficientFundsException), NotEnoughBalance},
                {typeof(InvalidGameActionException), UnableToProceedPleaseTryAgainLater},
                {typeof(InvalidAmountException), InvalidAmount},
                {typeof(ArgumentException), InvalidParameters},
                {typeof(ModelBindingException), Fail},
                {typeof(InvalidStatusException), Fail},
                {typeof(InvalidOperationException), UnableToProceedPleaseTryAgainLater}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {None, Fail},
                    { Duplicate, InvalidBet},
                    { Cancelled, BetHasCanceled},
                    { Settled, InvalidBet},
                    { InvalidGameActionErrorCode.TransactionNotFound,GameApiError. TransactionNotFound},
                    { TransactionBeingCancelledNotFound, InvalidBet},
                    { TransactionBeingReferencedNotFound, InvalidBet},
                    { RoundNotFound ,GameApiError.TransactionNotFound},
                    { GameNotFound, InvalidGamecode},
                    { ActionProhibited, InvalidBet}
                };

        int IErrorMapper.GetApiErrorByException(Exception exception)
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
            return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)Fail;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}
