using System;

namespace TGS.Services.Agent.Exceptions
{
    public class UsernameException : Exception
    {
        public UsernameException()
            : base("NameExisted")
        {
        }
    }
}