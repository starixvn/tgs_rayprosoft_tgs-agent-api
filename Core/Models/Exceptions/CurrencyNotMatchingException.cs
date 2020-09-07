using System;

namespace TGS.Core.Models.Exceptions
{
    public class CurrencyNotMatchingException : Exception
    {
        public CurrencyNotMatchingException() : base("Currency not matching ") { }

        public CurrencyNotMatchingException(string expected, string found) :
            base($"Currency not matching , expected: '{expected}' found:'{found}'")
        { }
    }
}