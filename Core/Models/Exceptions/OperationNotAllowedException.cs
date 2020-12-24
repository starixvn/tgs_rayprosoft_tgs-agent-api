using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class OperationNotAllowedException : Exception
    {
        public OperationNotAllowedException() : base("The operation is not allowed")
        {
        }

        public OperationNotAllowedException(string function) : base($"The operation of '{function}' is not allowed")
        {
        }
    }
}