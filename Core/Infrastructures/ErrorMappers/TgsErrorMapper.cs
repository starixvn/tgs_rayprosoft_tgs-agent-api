using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;

namespace TGS.Core.Infrastructures.ErrorMappers
{
    public sealed class TgsErrorMapper : IErrorMapper
    {
        internal static readonly IDictionary<Type, TgsApiError> TgsApiErrorByException =
            new Dictionary<Type, TgsApiError>
            {

                { typeof(PlayerDisabledException), TgsApiError.UserBlocked },
                { typeof(PlayerNotFoundException), TgsApiError.PlayerNotFound }, 
                { typeof(ArgumentException), TgsApiError.InvalidArguments },
                { typeof(FormatException), TgsApiError.IncorrectFormat },
                { typeof(InvalidAmountException), TgsApiError.InvalidAmount },
                { typeof(InvalidOperationException), TgsApiError.SystemError },
                { typeof(InvalidStatusException), TgsApiError.SystemError },
                { typeof(InvalidGameActionException), TgsApiError.InvalidGameAction },               
                { typeof(GameProviderUnderMaintenanceException), TgsApiError.UnderMaintenance },
                { typeof(GameUnderMaintenanceException), TgsApiError.UnderMaintenance }
            };

        internal static readonly IDictionary<TgsApiError, int> HttpCodeByException =
           new Dictionary<TgsApiError, int>
           {
               { TgsApiError.InvalidArguments, StatusCodes.Status500InternalServerError},
               { TgsApiError.InvalidCredentials, StatusCodes.Status401Unauthorized},
               { TgsApiError.PlayerNotFound, StatusCodes.Status401Unauthorized },
               { TgsApiError.TransactionIdNotFound, StatusCodes.Status500InternalServerError },
               { TgsApiError.DuplicateTransaction, StatusCodes.Status400BadRequest },
               { TgsApiError.CurrencyMismatch, StatusCodes.Status500InternalServerError },
               { TgsApiError.SystemError, StatusCodes.Status500InternalServerError },
               { TgsApiError.UnderMaintenance, StatusCodes.Status503ServiceUnavailable },
               { TgsApiError.OperationNotAllowed, StatusCodes.Status405MethodNotAllowed },
               { TgsApiError.CountryRestricted, StatusCodes.Status403Forbidden },
               { TgsApiError.TestPlayerStakeAmountCapExceeded, StatusCodes.Status500InternalServerError },               
           };

        int IErrorMapper.GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int)TgsApiError.None;
            }

            var type = exception.GetType();
            if (TgsApiErrorByException.TryGetValue(type, out var error))
            {
                return (int)error;
            }

            return (int)TgsApiError.SystemError;
        }

        int IErrorMapper.GetHttpStatusByException(Exception exception)
        {
            if (exception == null)
            {
                return StatusCodes.Status200OK;
            }

            var type = exception.GetType();
            if (TgsApiErrorByException.TryGetValue(type, out var error) && 
                HttpCodeByException.TryGetValue(error, out var code))
            {
                return code;
            }

            return StatusCodes.Status500InternalServerError;
        }

        string IErrorMapper.GetMessageByException(Exception exception)
        {
            return exception.Message;
        }
    }
}
