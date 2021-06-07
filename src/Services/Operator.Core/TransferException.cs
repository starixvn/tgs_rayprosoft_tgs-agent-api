using System;

namespace TGS.Services.Operator
{
    public class TransferException : Exception
    {
        public TransferException(string errorMessage)
            : base($"The Transfer has Error ({errorMessage}).")
        {
        }
    }
}