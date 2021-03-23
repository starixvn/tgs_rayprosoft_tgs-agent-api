using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class PlayerLoginExpiredException : Exception
    {
        public PlayerLoginExpiredException()
            : base("Player login token is expired")
        {
        }

        public PlayerLoginExpiredException(string playerCode)
            : base($"Player({playerCode}) login token is expired")
        {
        }
    }
}