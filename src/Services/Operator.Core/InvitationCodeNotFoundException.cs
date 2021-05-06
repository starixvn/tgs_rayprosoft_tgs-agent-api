using System;

namespace TGS.Services.Operator
{
    public class InvitationCodeNotFoundException : Exception
    {
        public InvitationCodeNotFoundException(string invitationCode)
            : base($"The invitation code ({invitationCode}) can't find the agent info.")
        {
        }
    }
}