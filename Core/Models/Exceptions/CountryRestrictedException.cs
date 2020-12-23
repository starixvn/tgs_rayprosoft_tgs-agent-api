using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class CountryRestrictedException : Exception
    {
        public CountryRestrictedException(string ipAddress) : base($"Player from {ipAddress} is not allowed.")
        {
        }
    }
}