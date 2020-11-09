using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string message, Exception exception = null) : base(message, exception)
        {
        }
    }
}
