using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class PlayerHasInsufficientFundsException : Exception
    {
        public PlayerHasInsufficientFundsException() : base("Player has insufficient funds")
        {
        }
    }
}