using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class InvalidInvitationCodeException : Exception
    {
        public InvalidInvitationCodeException(string invitationCode, string reason)
            : base($"Invalid invitation code: {invitationCode}. Error: {reason}")
        {
            
        }
    }
}
