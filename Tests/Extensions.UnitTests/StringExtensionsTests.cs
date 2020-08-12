using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace TKS.Infrastructures.Extensions.UnitTests
{
    [TestFixture, Category(Named.Unit), Parallelizable]
    public class StringExtensionsTests
    {
        [Test]
        public void Can_Throw_ArgumentException_If_Url_Is_Null()
        {
            Action action = () => StringExtensions.AppendQueryString(null, string.Empty);

            action.Should().Throw<ArgumentNullException>().Where(e => e.Message.Contains("url"));
        }

        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Can_Get_Unchanged_Url_If_QueryString_Is_Null_Or_Empty(string queryString)
        {
            const string url = "https://www.afusion.com";

            url.AppendQueryString(queryString).Should().Be(url);
        }

        [TestCase("https://www.afusion.com", "username=pete.chen",
            "https://www.afusion.com?username=pete.chen")]
        [TestCase("https://www.afusion.com?team=dotnet", "username=pete.chen",
            "https://www.afusion.com?team=dotnet&username=pete.chen")]
        public void Can_Append_QueryString(string url, string queryString, string result)
        {
            url.AppendQueryString(queryString).Should().Be(result);
        }

        [TestCase("T", "T")]
        [TestCase("T01", "T**")]
        [TestCase("TP01", "T**1")]
        [TestCase("TP00001", "TP0**01")]
        [TestCase("***00", "****0")]
        [TestCase("*00", "***")]
        [TestCase(" *00  ", "***")]
        public void Can_Mask_Plain_Text(string name, string result)
        {
            name.Mask().Should().Be(result);
        }

        private static readonly object[] DataCasesJoinAsString =
        {
            new object[] { ";", new [] { "A", "s", "i", "a" }, "A;s;i;a"},
            new object[] { " ", new [] { "F", "u", "s", "i", "o", "n" }, "F u s i o n"}
        };

        [TestCaseSource(nameof(DataCasesJoinAsString))]
        public void Check_Joint_String(string separator, string[] queryString, string result)
        {
            queryString.JoinAsString(separator).Should().Be(result);
        }

        private static readonly object[] DataCasesJoinAsStringByColon =
        {
            new object[] { new [] { "A", "s", "i", "a" }, "A:s:i:a"},
            new object[] { new [] { "F", "u", "s", "i", "o", "n" }, "F:u:s:i:o:n"}
        };

        [TestCaseSource(nameof(DataCasesJoinAsStringByColon))]
        public void Check_Joint_String_By_Colon(string[] queryString, string result)
        {
            queryString.JoinAsStringByColon().Should().Be(result);
        }

        private static readonly object[] DataCasesJoinAsStringBySemicolon =
        {
            new object[] { new [] { "A", "s", "i", "a" }, "A;s;i;a"},
            new object[] { new [] { "F", "u", "s", "i", "o", "n" }, "F;u;s;i;o;n"}
        };

        [TestCaseSource(nameof(DataCasesJoinAsStringBySemicolon))]
        public void Check_Joint_String_By_Semicolon(string[] queryString, string result)
        {
            queryString.JoinAsStringBySemicolon().Should().Be(result);
        }

        private static readonly object[] DataCasesJoinAsStringByComma =
        {
            new object[] { new [] { "A", "s", "i", "a" }, "A,s,i,a"},
            new object[] { new [] { "F", "u", "s", "i", "o", "n" }, "F,u,s,i,o,n"}
        };

        [TestCaseSource(nameof(DataCasesJoinAsStringByComma))]
        public void Check_Joint_String_By_Comma(string[] queryString, string result)
        {
            queryString.JoinAsStringByComma().Should().Be(result);
        }

        private static readonly object[] DataCasesJoinAsStringByUndescore =
        {
            new object[] { new [] { "A", "s", "i", "a" }, "A_s_i_a"},
            new object[] { new [] { "F", "u", "s", "i", "o", "n" }, "F_u_s_i_o_n"},
            new object[] { new [] { "Technology" }, "Technology" }
        };

        [TestCaseSource(nameof(DataCasesJoinAsStringByUndescore))]
        public void Check_Joint_String_By_Underscore(string[] queryString, string result)
        {
            queryString.JoinAsStringByUnderscore().Should().Be(result);
        }

        [TestCase("{0},{1}", new[] { "Asia", "Fusion" }, "Asia,Fusion")]
        public void Check_Args(string format, object[] queryString, string result)
        {
            format.Args(queryString).Should().Be(result);
        }

        [TestCase("")]
        [TestCase(null)]
        public void Can_Get_Empty_From_Split_By_Semicolon(string queryString)
        {
            queryString.SplitBySemicolon().Should().BeEquivalentTo(new string[0]);
        }

        [TestCase("A;s;i;a", new[] { "A", "s", "i", "a" })]
        public void Split_By_Semicolon(string queryString, string[] result)
        {
            queryString.SplitBySemicolon().Should().BeEquivalentTo(result);
        }

        [Test]
        [TestCase("")]
        public void Split_By_Semicolon_If_Null_Or_Empty(string queryString)
        {
            queryString.SplitBySemicolon().Should().BeEquivalentTo(new string[0]);
        }

        [TestCase("A:s:i:a", new[] { "A", "s", "i", "a" })]
        [TestCase("As:ia", new[] { "As", "ia" })]
        [TestCase("", new[] { "" })]
        public void Split_By_Colon(string queryString, string[] result)
        {
            queryString.SplitByColon().Should().BeEquivalentTo(result);
        }

        [Test]
        public void Throw_Exception_If_Arg_Is_Null_When_SplitByColon()
        {
            Assert.Throws(typeof(NullReferenceException), () => StringExtensions.SplitByColon(null));
        }

        [TestCase("A_s_i_a", new[] { "A", "s", "i", "a" })]
        [TestCase("As_ia", new[] { "As", "ia" })]
        [TestCase("", new[] { "" })]
        public void Split_By_Underscore(string queryString, string[] result)
        {
            queryString.SplitByUnderscore().Should().BeEquivalentTo(result);
        }

        [Test]
        public void Throw_Exception_If_Arg_Is_Null_When_SplitByUnderscore()
        {
            Assert.Throws(typeof(NullReferenceException), () => StringExtensions.SplitByUnderscore(null));
        }

        [Test]
        public void Can_Throw_Exception_If_Arg_Is_Null_When_SplitByComma()
        {
            Assert.Throws(typeof(NullReferenceException), () => StringExtensions.SplitByComma(null));
        }

        [TestCase("A,s,i,a", new[] { "A", "s", "i", "a" })]
        public void Can_Split_By_Comma(string queryString, string[] result)
        {
            queryString.SplitByComma().Should().BeEquivalentTo(result);
        }

        [TestCase("Company", 10)]
        public void Can_Get_Only_String_When_Length_Shorter_Than_Chars(string queryString, int chars)
        {
            queryString.NoLongerThan(chars).Should().Be(queryString);
        }

        [TestCase("Company", 3, "Com")]
        public void Can_Get_Valid_String_When_Length_Than_Chars(string queryString, int chars, string result)
        {
            queryString.NoLongerThan(chars).Should().Be(result);
        }

        [TestCase("")]
        [TestCase(null)]
        public void Can_Get_Unchanged_String_If_QueryString_Is_Null_Or_Empty(string queryString)
        {
            queryString.CamelCaseToWords().Should().Be(queryString);
        }

        [TestCase("AsiaFusion", "Asia Fusion")]
        public void Can_Get_CamelCase_To_Words(string queryString, string result)
        {
            queryString.CamelCaseToWords().Should().Be(result);
        }

        [Test]
        [TestCase(null)]
        public void Can_Get_True_If_QueryString_Is_NULL_OR_Empty(string queryString)
        {
            queryString.IsNullOrEmpty().Should().Be(true);
        }

        [TestCase("Asia")]
        [TestCase("Asia Fusion")]
        public void Can_Get_True_If_QueryString_Is_Not_NULL_OR_Empty(string queryString)
        {
            queryString.IsNotNullOrEmpty().Should().Be(true);
        }

        [TestCase("Asia Fusion", "Asia Fusion")]
        [TestCase("/Asia Fusion", "Asia Fusion")]
        public void Can_Get_Correct_String_By_TrimStartSlash(string queryString, string result)
        {
            queryString.TrimStartSlash().Should().Be(result);
        }

        [TestCase("Asia Fusion", "Asia Fusion")]
        [TestCase("Asia Fusion/", "Asia Fusion")]
        public void Can_Get_Correct_String_By_TrimEndSlash(string queryString, string result)
        {
            queryString.TrimEndSlash().Should().Be(result);
        }

        [TestCase("abc 753", "abc_753")]
        public void Can_Replace_Space_By_Underscore(string queryString, string result)
        {
            queryString.ReplaceSpaceByUnderscore().Should().Be(result);
        }

        [TestCase("abc 753", "abc753")]
        public void Can_Replace_Space_By_Empty(string queryString, string result)
        {
            queryString.ReplaceSpaceByEmpty().Should().Be(result);
        }

        [TestCase("abc'753", "abc753")]
        public void Can_Replace_Apostrophe_By_Empty(string queryString, string result)
        {
            queryString.ReplaceApostropheByEmpty().Should().Be(result);
        }

        [TestCase("abc\"\"753", "abc\"753")]
        public void Can_Replace_DoubleQuotationMark_By_SingleQuotationMark(string queryString, string result)
        {
            queryString.ReplaceDoubleQuotationMarkBySingleQuotationMark().Should().Be(result);
        }

        [TestCase("abc\"753", "abc753")]
        public void Can_Remove_DoubleQuote(string queryString, string result)
        {
            queryString.RemoveDoubleQuotes().Should().Be(result);
        }

        private static readonly object[] DataCasesJoinAsStringByCommaIfNotNull =
        {
            new object[] { new [] { "A", "s", "i", "a" }, "A,s,i,a"},
            new object[] { new string [] { }, null},
            new object[] { null, null}
        };

        [TestCaseSource(nameof(DataCasesJoinAsStringByCommaIfNotNull))]
        public void Can_JoinByCommaIfNotNull(string[] queryString, string result)
        {
            queryString.JoinAsStringByCommaIfNotNull().Should().Be(result);
        }

        [TestCase("STRING", "string", ExpectedResult = true)]
        [TestCase("string", "STRING", ExpectedResult = true)]
        [TestCase("string", "INGSTR", ExpectedResult = false)]
        public bool Check_IgnoreCaseEquals(string firstString, string secondString)
        {
            return firstString.IgnoreCaseEquals(secondString);
        }

        [TestCase("STRING", "string", ExpectedResult = false)]
        [TestCase("string", "STRING", ExpectedResult = false)]
        [TestCase("string", "INGSTR", ExpectedResult = true)]
        public bool Check_IgnoreCaseNotEquals(string firstString, string secondString)
        {
            return firstString.IgnoreCaseNotEquals(secondString);
        }

        [TestCase(new[] { "Asia", "Fusion", "Technology" }, "Asia", ExpectedResult = true)]
        [TestCase(new[] { "ASIA", "Fusion", "Technology" }, "Asia", ExpectedResult = true)]
        [TestCase(new[] { "AsiaFusion", "Technology" }, "Asia", ExpectedResult = false)]
        public bool Check_IgnoreCaseContains(IEnumerable<string> enumerable, string stringToFind)
        {
            return enumerable.IgnoreCaseContains(stringToFind);
        }

        [TestCase(new[] { "Asia", "Fusion", "Technology" }, "Asia", ExpectedResult = false)]
        [TestCase(new[] { "ASIA", "Fusion", "Technology" }, "Asia", ExpectedResult = false)]
        [TestCase(new[] { "AsiaFusion", "Technology" }, "Asia", ExpectedResult = true)]
        public bool Check_IgnoreCaseNotContains(IEnumerable<string> enumerable, string stringToFind)
        {
            return enumerable.IgnoreCaseNotContains(stringToFind);
        }

        [TestCase("STRING", "str", ExpectedResult = true)]
        [TestCase("string", "STR", ExpectedResult = true)]
        [TestCase("string", "INGSTR", ExpectedResult = false)]
        public bool Check_IgnoreCaseIndexOf(string firstString, string secondString)
        {
            return firstString.IgnoreCaseContains(secondString);
        }
    }
}