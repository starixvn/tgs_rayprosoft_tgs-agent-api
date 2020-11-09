using System;

namespace TGS.Core.Models.Exceptions
{
    public class MissingTokenException : Exception
    {
        public MissingTokenException() : base("Missing Token")
        {
        }

        public MissingTokenException(string message, Exception exception = null) : base(message, exception)
        {
        }
    }
}