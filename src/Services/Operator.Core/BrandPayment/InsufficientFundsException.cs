using System;
using TGS.Services.Operator;

namespace TGS.Services.Operator.BrandPayment
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException()
        {
        }
    }
}
