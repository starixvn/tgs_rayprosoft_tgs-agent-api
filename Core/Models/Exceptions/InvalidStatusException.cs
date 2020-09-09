using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class InvalidStatusException : Exception
    {
        public InvalidStatusException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}