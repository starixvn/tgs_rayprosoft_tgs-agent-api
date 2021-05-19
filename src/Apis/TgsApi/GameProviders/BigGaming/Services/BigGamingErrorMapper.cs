using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Shared.Game.Models.BigGaming.Enums;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;
using static TGS.Shared.Game.Models.BigGaming.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public sealed class BigGamingErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, GameApiError> ApiErrorByException =
            new Dictionary<Type, GameApiError>
            {
                {typeof(InvalidCredentialsException), Authentication_Sign_Is_Invalid},
                {typeof(MissingTokenException), Authentication_Sign_Is_Invalid},
                {typeof(PlayerLoginExpiredException), Authentication_Sign_Is_Invalid},
                {typeof(PlayerNotFoundException), Account_Does_Not_Exist},
                {typeof(PlayerDisabledException), Current_User_Has_Been_Banned_And_Login_Failed},
                {typeof(PlayerHasInsufficientFundsException), Insufficient_Betting_Balance},
                {typeof(InvalidGameActionException), Unknown_Data_Access_Exception},
                {typeof(InvalidAmountException), Abnormal_Flow_Of_Bulk_Funds_Into_The_Warehouse},
                {typeof(ArgumentException), Encryption_And_Decryption_Data_Is_Abnormal},
                {typeof(ModelBindingException), Parameters_Passed_In_Are_Incorrect},
                {typeof(InvalidStatusException), Unknown_Exception},
                {typeof(InvalidOperationException), Unknown_Exception}
            };

        private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, GameApiError>
            ApiErrorByInvalidGameActionErrorCode =
                new Dictionary<InvalidGameActionErrorCode, GameApiError>
                {
                    {TransactionNotFound, Bet_Slip_Does_Not_Exist},
                    {TransactionBeingCancelledNotFound, Bet_Slip_Does_Not_Exist},
                    {TransactionBeingReferencedNotFound, Bet_Slip_Does_Not_Exist},
                    {RoundNotFound, Bet_Slip_Does_Not_Exist},
                    {GameNotFound, Bet_Slip_Does_Not_Exist}
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
            return ApiErrorByException.TryGetValue(type, out error) ? (int)error : (int)Unknown_Exception;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception) =>
            StatusCodes.Status200OK;

        string IErrorMapper.GetMessageByException(Exception exception) =>
            exception.Message;
    }
}
