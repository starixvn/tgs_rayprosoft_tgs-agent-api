using System;

namespace TGS.Services.Operator
{
    public class LeePayApiException : Exception
    {
        public LeePayApiException()
            : base($"LeePay api has error.")
        {
        }
    }
}