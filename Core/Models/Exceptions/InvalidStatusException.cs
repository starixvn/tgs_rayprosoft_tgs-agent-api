using System;

namespace TGS.Core.Models.Exceptions
{
    public class InvalidStatusException : Exception
    {
        public InvalidStatusException() : base("Invalid Status")
        {
        }

        public InvalidStatusException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}