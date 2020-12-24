using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class GameProviderApiErrorException : Exception
    {
        public GameProviderApiErrorException(string message) : base(message)
        {
        }
    }
}