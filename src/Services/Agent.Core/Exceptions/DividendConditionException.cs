using System;

namespace TGS.Services.Agent.Exceptions
{
    public class DividendConditionException : Exception
    {
        public DividendConditionException(string? message = null)
            : base(message)
        {
        }
    }
}