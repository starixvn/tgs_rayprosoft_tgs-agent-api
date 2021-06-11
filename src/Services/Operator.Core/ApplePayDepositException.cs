using System;

namespace TGS.Services.Operator
{
    public class ApplePayDepositException : Exception
    {
        public ApplePayDepositException(string errorMessage)
            : base($"ApplePay deposit has error({errorMessage}).")
        {
        }
    }
}