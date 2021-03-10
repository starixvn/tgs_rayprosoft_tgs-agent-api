using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class PlayerIdAlreadyValidatedException : Exception
    {
        public PlayerIdAlreadyValidatedException() : base("Id is already validated.")
        {
        }
    }
}