using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TGS.Core.Infrastructures.Extensions
{
    public static class StringExtensions
    {
        public static string JoinAsStringByColon<T>(this IEnumerable<T> enumerable)
            => string.Join(":", enumerable);

        public static string JoinAsStringBySemicolon<T>(this IEnumerable<T> enumerable)
            => string.Join(";", enumerable);

        public static string JoinAsStringByComma<T>(this IEnumerable<T> enumerable)
            => string.Join(",", enumerable);

        public static string JoinAsStringByUnderscore<T>(this IEnumerable<T> enumerable)
            => string.Join("_", enumerable);

        public static string JoinAsString<T>(this IEnumerable<T> enumerable, string separator)
            => string.Join(separator, enumerable);

        public static string JoinAsStringByCommaIfNotNull<T>(this T[] strs)
            => strs?.Length > 0 ? strs.JoinAsStringByComma() : null;

        public static string Args(this string str, params object[] args)
            => string.Format(str, args);

        public static string[] SplitBySemicolon(this string s, StringSplitOptions options = StringSplitOptions.None)
            => s.Split(';', options);

        public static string[] SplitByColon(this string s, StringSplitOptions options = StringSplitOptions.None)
            => s.Split(':', options);

        public static string[] SplitByComma(this string s, StringSplitOptions options = StringSplitOptions.None)
            => s.Split(',', options);

        public static string[] SplitByUnderscore(this string s, StringSplitOptions options = StringSplitOptions.None)
            => s.Split('_', options);

        public static string[] SplitByEqualsSign(this string s, StringSplitOptions options = StringSplitOptions.None)
            => s.Split("=", options);

        public static string AppendQueryString(this string url, string queryString)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (string.IsNullOrWhiteSpace(queryString))
            {
                return url;
            }

            if (queryString.StartsWith("&") || queryString.StartsWith("?"))
            {
                queryString = queryString[1..];
            }

            var prefix = url.IndexOf('?') >= 0 ? "&" : "?";
            return $"{url}{prefix}{queryString}";
        }

        public static string NoLongerThan(this string str, int chars)
            => string.IsNullOrEmpty(str) || str.Length <= chars ? str : str.Substring(0, chars);

        public static string CamelCaseToWords(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            var sb = new StringBuilder();
            char? prevChar = null;
            foreach (var curr in str)
            {
                if (prevChar != null)
                {
                    var prev = prevChar.Value;

                    if (char.IsLower(prev) && char.IsUpper(curr)) sb.Append(' ');
                }

                sb.Append(curr);
                prevChar = curr;
            }

            return sb.ToString();
        }

        public static bool IsNullOrEmpty(this string str)
            => string.IsNullOrEmpty(str);

        public static bool IsNotNullOrEmpty(this string str)
            => !string.IsNullOrEmpty(str);

        public static string TrimStartSlash(this string str)
            => str.TrimStart('/');

        public static string TrimEndSlash(this string str)
            => str.TrimEnd('/');

        public static string ReplaceSpaceByUnderscore(this string str)
            => str.Replace(" ", "_");

        public static string ReplaceSpaceByEmpty(this string str)
            => str.Replace(" ", string.Empty);

        public static string ReplaceApostropheByEmpty(this string str)
            => str.Replace("'", string.Empty);

        public static string ReplaceDoubleQuotationMarkBySingleQuotationMark(this string str)
            => str.Replace("\"\"", "\"");

        public static string RemoveDoubleQuotes(this string str)
            => str.Replace("\"", string.Empty);

        public static string Mask(this string str)
        {
            str = str.Trim();
            return str.Length > 5 ? string.Concat(str.Substring(0, 3), "".PadLeft(str.Length - 5, '*'),
                    str.Substring(str.Length - 2)) :
                str.Length > 3 ? string.Concat(str.Substring(0, 1), "".PadLeft(str.Length - 2, '*'),
                                     str.Substring(str.Length - 1)) :
                                 string.Concat(str.Substring(0, 1), "".PadLeft(str.Length - 1, '*'));
        }

        public static bool NotEquals(this string str, string second)
            => !string.Equals(str, second);

        public static bool IgnoreCaseEquals(this string str, string second)
            => string.Equals(str, second, StringComparison.OrdinalIgnoreCase);

        public static bool IgnoreCaseNotEquals(this string str, string second)
            => !str.IgnoreCaseEquals(second);

        public static bool IgnoreCaseContains(this IEnumerable<string> enumerable, string second)
            => enumerable.Contains(second, StringComparer.OrdinalIgnoreCase);

        public static bool IgnoreCaseNotContains(this IEnumerable<string> enumerable, string second)
            => !enumerable.IgnoreCaseContains(second);

        public static bool IgnoreCaseContains(this string str, string second) =>
            str.Contains(second, StringComparison.OrdinalIgnoreCase);

        public static bool IgnoreCaseNotContains(this string str, string second) =>
            !str.IgnoreCaseContains(second);
    }
}