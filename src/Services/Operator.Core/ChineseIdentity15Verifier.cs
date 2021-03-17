using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace TGS.Services.Operator
{
    public class ChineseIdentity15Verifier
    {
        public bool Verify(string id)
        {
            var regex = new Regex(@"\d{15}");

            if (!regex.IsMatch(id))
                return false;

            var cityCodes = new[]
            {
                "11", "12", "13", "14", "15", "21", "22", "23", "31", "32", "33", "34", "35", "36", "37", "41",
                "42", "43", "44", "45", "46", "50", "51", "52", "53", "54", "61", "62", "63", "64", "65", "71",
                "81", "82", "91"
            };

            var areaCode = id.Substring(0, 6);
            var birthday = "19" + id.Substring(6, 6);
            var sequence = id.Substring(12, 3);

            if (!cityCodes.Contains(areaCode.Substring(0, 2)))
                return false;

            if (!DateTime.TryParseExact(
                birthday,
                "yyyyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
                return false;

            return true;
        }
    }
}
