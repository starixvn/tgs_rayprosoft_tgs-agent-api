using System;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Exceptions
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message = "Invalid Token") : base(message)
        {
        }
    }
}