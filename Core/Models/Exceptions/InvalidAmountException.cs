using System;

namespace TGS.Core.Models.Exceptions
{
    public class InvalidAmountException : Exception
    {
        public InvalidAmountException() : base("Invalid Amount") { }
        public InvalidAmountException(string details) : base($"Invalid Amount: {details}") { }
    }
}
