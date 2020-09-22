using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class PlayerLoginExpiredException : Exception
    {
        public PlayerLoginExpiredException(string playerCode) : base($"Player({playerCode}) login record is expired") { }
    }
}