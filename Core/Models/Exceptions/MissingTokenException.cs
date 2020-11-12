using System;

namespace TGS.Core.Models.Exceptions
{
    public class MissingTokenException : Exception
    {
        public MissingTokenException(string message = "Missing Token", Exception exception = null) : base(message, exception)
        {
        }
    }
}