using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Exceptions
{
    public sealed class InvalidGameActionException : Exception
    {
        public InvalidGameActionErrorCode ErrorCode { get; } = InvalidGameActionErrorCode.None;

        public InvalidGameActionException(string message, InvalidGameActionErrorCode errorCode = InvalidGameActionErrorCode.None) : base(message)
        {
            ErrorCode = errorCode;
        }

        public InvalidGameActionException(string message, Exception ex, InvalidGameActionErrorCode errorCode = InvalidGameActionErrorCode.None) : 
            base(message, ex)
        {
            ErrorCode = errorCode;
        }
    }
}