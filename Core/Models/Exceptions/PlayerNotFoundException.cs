using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class PlayerNotFoundException : Exception
    {        
        public PlayerNotFoundException(string playerCode) 
            : base($"Player not found with code: {playerCode}") { }

        public PlayerNotFoundException(string userKey, string message)
            : base(string.Format(message, userKey)) { }
    }
}