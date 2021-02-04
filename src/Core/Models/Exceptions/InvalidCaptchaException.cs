using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class InvalidCaptchaException : Exception
    {
        public InvalidCaptchaException(string message) : base(message)
        {
                
        }
    }
}
