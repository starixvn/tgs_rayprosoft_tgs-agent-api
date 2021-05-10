using System;

namespace TGS.Services.Operator
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(string email)
            : base($"The email ({email}) can't find the player info.")
        {
        }
    }
}