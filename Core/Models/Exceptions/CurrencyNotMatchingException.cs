using System;

namespace TGS.Core.Models.Exceptions
{
    public sealed class CurrencyNotMatchingException : Exception
    {
        public CurrencyNotMatchingException(string expected, string found) :
            base($"Currency not matching , expected: '{expected}' found:'{found}'")
        {
        }
    }
}