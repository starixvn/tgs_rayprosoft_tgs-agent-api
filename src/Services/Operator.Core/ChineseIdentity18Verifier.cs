using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public class ChineseIdentity18Verifier
    {
        public bool Verify(string id)
        {
            var regex = new Regex(@"\d{17}\d|X");

            if (!regex.IsMatch(id))
                return false;

            var cityCodes = new[]
            {
                "11", "12", "13", "14", "15", "21", "22", "23", "31", "32", "33", "34", "35", "36", "37", "41",
                "42", "43", "44", "45", "46", "50", "51", "52", "53", "54", "61", "62", "63", "64", "65", "71",
                "81", "82", "91"
            };

            var weights = new[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };

            var areaCode = id.Substring(0, 6);
            var birthday = id.Substring(6, 8);
            var sequence = id.Substring(12, 3);
            var checkCode = id.Last();

            if (!cityCodes.Contains(areaCode.Substring(0, 2)))
                return false;

            if (!DateTime.TryParseExact(
                birthday,
                "yyyyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
                return false;

            var sum = id[0..^1].Zip(weights).Select(t => (t.First - '0') * t.Second).Sum();
            var proof = (12 - sum % 11) % 11;

            if (proof < 10)
                return proof == checkCode - '0';
            else
                return checkCode == 'X';
        }
    }
}
