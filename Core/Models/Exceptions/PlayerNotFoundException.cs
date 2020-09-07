using System;

namespace TGS.Core.Models.Exceptions
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException() : base("Player not found") { }
        public PlayerNotFoundException(string message) : base(message) { }
    }
}