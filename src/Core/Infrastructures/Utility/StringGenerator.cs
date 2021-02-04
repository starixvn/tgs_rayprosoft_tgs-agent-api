using System;
using System.Linq;

namespace TGS.Core.Infrastructures.Utility
{
    public static class StringGenerator
    {
        private const string AlphanumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
        private static readonly Random Random = new Random();

        public static string GenerateAlphanumericString(int length)
        {
            return GenerateString(AlphanumericCharacters, length);
        }

        private static string GenerateString(string chars, int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Length should be greater then zero");
            }

            var result = Enumerable
                .Range(1, length)
                .Select(n => chars[Random.Next(0, chars.Length)])
                .ToArray();
            return new string(result);
        }
    }
}
