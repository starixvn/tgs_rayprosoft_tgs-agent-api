using System;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Exceptions
{
    public class EmptyUsernameException : Exception
    {
        public EmptyUsernameException() : base("Username is empty")
        {
        }
    }
}