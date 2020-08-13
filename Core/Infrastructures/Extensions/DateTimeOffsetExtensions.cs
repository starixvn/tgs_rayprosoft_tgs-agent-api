using System;

namespace TGS.Infrastructures.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static string ToIso8601String(this DateTimeOffset dateTimeOffset)
        {
            // Returns string format like 2014-09-25T07:07:31+00:00
            return dateTimeOffset.ToString(@"yyyy-MM-dd\THH:mm:ssK");
        }

        public static string ToDateTimeFormat(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString(@"yyyy-MM-dd HH:mm:ss");
        }

        public static string ToUtcDateTimeFormat(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString(@"yyyy-MM-dd HH:mmK");
        }
    }
}