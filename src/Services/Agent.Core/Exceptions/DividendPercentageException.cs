using System;

namespace TGS.Services.Agent.Exceptions
{
    public class DividendPercentageException : Exception
    {
        public DividendPercentageException(string message)
            : base(message)
        {
        }
    }
}
