using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Interfaces;
using TGS.Services.Operator;

namespace TGS.Apis.OperatorApi.Models
{
    public class OperatorApiErrorMapper : IErrorMapper
    {
        private static readonly IReadOnlyDictionary<Type, TgsApiError> TgsApiErrorByException =
            new Dictionary<Type, TgsApiError>
            {
                [typeof(ValidateCodeNotVerifyException)] = TgsApiError.ValidateCodeNotVerify,
                [typeof(PlayerNotFoundException)] = TgsApiError.PlayerNotFound
            };

        private static readonly IReadOnlyDictionary<TgsApiError, int> HttpCodeByException =
            new Dictionary<TgsApiError, int>
            {
                [TgsApiError.ValidateCodeNotVerify] = StatusCodes.Status406NotAcceptable,
                [TgsApiError.PlayerNotFound] = StatusCodes.Status404NotFound
            };

        public int GetApiErrorByException(Exception exception)
        {
            if (exception == null)
            {
                return (int)TgsApiError.None;
            }

            var type = exception.GetType();
            return TgsApiErrorByException.TryGetValue(type, out var error) ? (int)error : (int)TgsApiError.SystemError;
        }

        public int GetHttpStatusByException(Exception exception)
        {
            if (exception == null)
            {
                return StatusCodes.Status200OK;
            }

            var type = exception.GetType();
            return TgsApiErrorByException.TryGetValue(type, out var error) &&
                HttpCodeByException.TryGetValue(error, out var statusCode)
                    ? statusCode
                    : StatusCodes.Status500InternalServerError;
        }

        public string GetMessageByException(Exception exception) =>
            exception.Message;

        public bool CanProcess(Exception exception)
        {
            return TgsApiErrorByException.TryGetValue(exception.GetType(), out _);
        }
    }
}