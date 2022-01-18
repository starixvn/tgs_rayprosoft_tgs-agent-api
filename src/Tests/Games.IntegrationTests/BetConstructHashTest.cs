using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace TGS.Tests.Games.IntegrationTests
{
    public class BetConstructHashTest
    {
        [Test]
        public void Case_Millsecond_Format_With_One_Length()
        {
            var created = new DateTime(2022, 01, 17, 5, 28, 30, 600);

            var millsecondLength = created.Millisecond
                .ToString()
                .TrimEnd('0')
                .Length;

            var result = string.Empty;

            for (var i = 0; i < millsecondLength; i++)
            {
                result += "f";
            }

            var expected = "f";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Case_Millsecond_Format_With_Three_Length()
        {
            var created = new DateTime(2022, 01, 17, 5, 28, 30, 551);

            var millsecondLength = created.Millisecond
                .ToString()
                .TrimEnd('0')
                .Length;

            var result = string.Empty;

            for (var i = 0; i < millsecondLength; i++)
            {
                result += "f";
            }

            var expected = "fff";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Case_Millsecond_Format_With_Two_Length()
        {
            var created = new DateTime(2022, 01, 17, 5, 28, 30, 690);

            var millsecondLength = created.Millisecond
                .ToString()
                .TrimEnd('0')
                .Length;

            var result = string.Empty;

            for (var i = 0; i < millsecondLength; i++)
            {
                result += "f";
            }

            var expected = "ff";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Test_BC_Hash_With_Two_Millsecond()
        {
            var authToken = "xoyHnP7Nv0THjlNuyaYac2";
            var ts = "1642397310";
            var transactionId = "55180791226";
            var betId = "1929756140";
            var amount = "50.00";
            var created = new DateTime(2022, 01, 17, 5, 28, 30, 690);
            var betType = 1;
            var systemMinCount = new Nullable<int>();
            var totalPrice = 1.870m;

            var trimZeroLength = created.Millisecond.ToString().TrimEnd('0').Length;

            var formatResult = string.Empty;
            for (var i = 0; i < trimZeroLength; i++)
            {
                formatResult += "f";
            }

            var hashDict = new Dictionary<string, object>
            {
                { "AuthToken" , authToken },
                { "TS", ts },
                { "TransactionId", transactionId},
                { "BetId", betId},
                { "Amount", amount },
                { "Created", created.ToString($"yyyy-MM-ddTHH:mm:ss.{formatResult}Z")},
                { "BetType", betType},
                { "SystemMinCount", systemMinCount},
                { "TotalPrice", totalPrice}
            };

            var content = string.Join(
                            "",
                            hashDict.Where(x => x.Value != null && x.Value.ToString() != "")
                                .Select(x =>
                                {
                                    return $"{x.Key}{x.Value}";
                                }));

            var sharedKey = "jiLEPYw5n3n5kcx6T3";

            var plainText = $"{content}{sharedKey}";

            var data = Encoding.UTF8.GetBytes(plainText);

            var hash = MD5.Create().ComputeHash(data);

            var result = Convert.ToHexString(hash).ToLower();

            var expected = "b724a16da8d70156877873cee85c2675";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Test_BC_Hash1()
        {
            var authToken = "xoyHnP7Nv0THjlNuyaYac2";
            var ts = "1642396398";
            var transactionId = "55180215412";
            var betId = "1929750873";
            var amount = "100.00";
            var created = new DateTime(2022, 01, 17, 5, 13, 17, 960);
            var betType = 1;
            var systemMinCount = new Nullable<int>();
            var totalPrice = 1.350m;

            var trimZeroLength = created.Millisecond.ToString().TrimEnd('0').Length;

            var formatResult = string.Empty;
            for (var i = 0; i < trimZeroLength; i++)
            {
                formatResult += "f";
            }

            var hashDict = new Dictionary<string, object>
            {
                { "AuthToken" , authToken },
                { "TS", ts },
                { "TransactionId", transactionId},
                { "BetId", betId},
                { "Amount", amount },
                { "Created", created.ToString($"yyyy-MM-ddTHH:mm:ss.{formatResult}Z")},
                { "BetType", betType},
                { "SystemMinCount", systemMinCount},
                { "TotalPrice", totalPrice}
            };

            var content = string.Join(
                            "",
                            hashDict.Where(x => x.Value != null && x.Value.ToString() != "")
                                .Select(x =>
                                {
                                    return $"{x.Key}{x.Value}";
                                }));

            var sharedKey = "jiLEPYw5n3n5kcx6T3";

            var plainText = $"{content}{sharedKey}";

            var data = Encoding.UTF8.GetBytes(plainText);

            var hash = MD5.Create().ComputeHash(data);

            var result = Convert.ToHexString(hash).ToLower();

            var expected = "64b6c439dc543a0f0d6dc178883b4a78";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Test_BC_Hash2()
        {
            var authToken = "xoyHnP7Nv0THjlNuyaYac2";
            var ts = "1642396339";
            var transactionId = "55180179784";
            var betId = "1929750550";
            var amount = "100.00";
            var created = new DateTime(2022, 01, 17, 5, 12, 19, 499);
            var betType = 2;
            var systemMinCount = new Nullable<int>();
            var totalPrice = 5.453m;

            var trimZeroLength = created.Millisecond.ToString().TrimEnd('0').Length;

            var formatResult = string.Empty;
            for (var i = 0; i < trimZeroLength; i++)
            {
                formatResult += "f";
            }

            var hashDict = new Dictionary<string, object>
            {
                { "AuthToken" , authToken },
                { "TS", ts },
                { "TransactionId", transactionId},
                { "BetId", betId},
                { "Amount", amount },
                { "Created", created.ToString($"yyyy-MM-ddTHH:mm:ss.{formatResult}Z")},
                { "BetType", betType},
                { "SystemMinCount", systemMinCount},
                { "TotalPrice", totalPrice}
            };

            var content = string.Join(
                            "",
                            hashDict.Where(x => x.Value != null && x.Value.ToString() != "")
                                .Select(x =>
                                {
                                    return $"{x.Key}{x.Value}";
                                }));

            var sharedKey = "jiLEPYw5n3n5kcx6T3";

            var plainText = $"{content}{sharedKey}";

            var data = Encoding.UTF8.GetBytes(plainText);

            var hash = MD5.Create().ComputeHash(data);

            var result = Convert.ToHexString(hash).ToLower();

            var expected = "840dab8d372a7c9695da93bd82a25e7d";
            Assert.AreEqual(expected, result);
        }
    }
}
