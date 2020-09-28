using System;

namespace TGS.Core.Infrastructures.Extensions
{
    public class ClientIdNotFoundException : Exception
    {
        public ClientIdNotFoundException(string message) : base(message)
        {
        }
    }
}