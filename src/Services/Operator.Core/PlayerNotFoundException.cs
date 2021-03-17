using System;

namespace TGS.Services.Operator
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(string phone)
            : base($"The phone ({phone}) can't find the player info.")
        {
        }
    }
}
