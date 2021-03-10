using System;
using System.Linq;

namespace TGS.Core.Infrastructures.Utility
{
    public static class IdVerification
    {
        public static bool CNYIdVerify(this string id)
        {
            if (id == null || id.Length <= 0)
            {
                return false;
            }

            var cityCodes = new[]
            {
                "11", "12", "13", "14", "15", "21", "22", "23", "31", "32", "33", "34", "35", "36", "37", "41",
                "42", "43", "44", "45", "46", "50", "51", "52", "53", "54", "61", "62", "63", "64", "65", "71",
                "81", "82", "91"
            };
            var parityBit = new[] {'1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2'};
            var power = new[] {7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2};

            if (id.Length == 15)
            {
                id = id.Substring(0, 6) + "19" + id.Substring(6, 10);
            }

            if (id.Length != 15 &&
                id.Length != 18 ||
                cityCodes.All(x => x != id.Substring(0, 2)) ||
                !DateTimeOffset.TryParse($"{id.Substring(6, 4)}/{id.Substring(10, 2)}/{id.Substring(12, 2)}",
                    out var birth) ||
                DateTimeOffset.Now.Year - birth.Year > 150 ||
                DateTimeOffset.Now < birth)
            {
                return false;
            }

            var sum = 0;

            var chars = id.ToCharArray();
            for (var i = 0; i < chars.Length - 1; i++)
            {
                sum += (chars[i] - '0') * power[i];
            }

            return parityBit[sum % 11] == chars[17];
        }
    }
}