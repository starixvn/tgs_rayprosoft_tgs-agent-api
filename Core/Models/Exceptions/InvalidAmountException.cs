using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class InvalidAmountException : Exception
    {
        public InvalidAmountException(string details) : base($"Invalid Amount: {details}") { }
    }
}
