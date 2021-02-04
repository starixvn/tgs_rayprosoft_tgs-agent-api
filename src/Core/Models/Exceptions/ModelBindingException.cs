using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class ModelBindingException : Exception
    {
        public ModelBindingException(string message) : base(message)
        {
        }
    }
}
