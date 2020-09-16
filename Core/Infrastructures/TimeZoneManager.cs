using System;
using TimeZoneConverter;

namespace TGS.Core.Infrastructures
{
    public interface ITimeZoneManager
    {
        DateTime UtcToLocal(string timeZone, DateTime utcTime);
        DateTimeOffset ToLocal(string timeZone, DateTimeOffset dateTimeOffset);
        DateTimeOffset ToUtc(string timeZone, DateTime dateTime);
        string GetTimeZoneNameByTimeZoneId(string timeZone);
    }

    public sealed class TimeZoneManager : ITimeZoneManager
    {
        DateTime ITimeZoneManager.UtcToLocal(string timeZone, DateTime utcTime)
        {
            if (utcTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"The date {utcTime} is not marked as UTC");
            }

            if (TZConvert.TryGetTimeZoneInfo(timeZone, out var tz))
            {
                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, tz);
            }

            throw new ArgumentException($"Unknown time zone {timeZone}");
        }

        DateTimeOffset ITimeZoneManager.ToLocal(string timeZone, DateTimeOffset dateTimeOffset)
        {
            if (TZConvert.TryGetTimeZoneInfo(timeZone, out var tz))
            {
                return TimeZoneInfo.ConvertTime(dateTimeOffset, tz);
            }

            throw new ArgumentException($"Unknown time zone {timeZone}");
        }

        DateTimeOffset ITimeZoneManager.ToUtc(string timeZone, DateTime dateTime)
        {
            if (TZConvert.TryGetTimeZoneInfo(timeZone, out var tz))
            {
                return new DateTimeOffset(dateTime, tz.BaseUtcOffset);
            }

            throw new ArgumentException($"Unknown time zone {timeZone}");
        }

        string ITimeZoneManager.GetTimeZoneNameByTimeZoneId(string timeZone)
        {
            if (TZConvert.TryGetTimeZoneInfo(timeZone, out var tz))
            {
                return tz.DisplayName;
            }

            throw new ArgumentException($"Unknown time zone {timeZone}");
        }
    }
}