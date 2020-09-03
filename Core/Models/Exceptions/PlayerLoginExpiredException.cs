using System;

namespace UGS.Core.Models.Exceptions
{
    public class PlayerLoginExpiredException : Exception
    {
        public PlayerLoginExpiredException(string playerCode) : base($"Player({playerCode}) login record is expired") { }
    }
}