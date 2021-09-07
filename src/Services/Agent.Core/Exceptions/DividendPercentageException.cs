using System;

namespace TGS.Services.Agent.Exceptions
{
    public class DividendPercentageException : Exception
    {
        public DividendPercentageException(decimal dividendPercentage)
            : base($"Dividend percentage can't less than {dividendPercentage}.")
        {
        }
    }
}