using System;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Exceptions
{
    public class InvalidOperatorIdException : Exception
    {
        public InvalidOperatorIdException(string message) : base(message)
        {
        }
    }
}