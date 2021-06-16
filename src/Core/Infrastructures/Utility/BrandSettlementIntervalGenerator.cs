using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Infrastructures.Utility
{
    public static class BrandSettlementIntervalGenerator
    {
        public static (DateTimeOffset? BeforeFromDate, DateTimeOffset? BeforeToDate, DateTimeOffset? AfterFromDate,
            DateTimeOffset? AfterToDate) GetInterval(
                SettlementIntervalType? type, DateTimeOffset? startDate = null)
        {
            var timeSpan = new TimeSpan(8, 0, 0);
            var now = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month,
                DateTimeOffset.Now.Day, 0,
                0, 0, timeSpan);

            if (startDate != null)
            {
                var date = startDate.Value.ToOffset(timeSpan);
                now = new(date.Year, date.Month,
                    date.Day, 0,
                    0, 0, timeSpan);
            }

            switch (type)
            {
                case SettlementIntervalType.Daily:
                    return (now, now, now.AddDays(1), now.AddDays(1));
                case SettlementIntervalType.Weekly:
                    return (now,
                        now.Subtract(new TimeSpan((int)now.DayOfWeek - 7, 0, 0, 0)),
                        now.Subtract(new TimeSpan((int)now.DayOfWeek - 8, 0, 0, 0)),
                        now.Subtract(new TimeSpan((int)now.DayOfWeek - 14, 0, 0, 0)));
                case SettlementIntervalType.Monthly:
                    var nextMonth = now.AddMonths(1);
                    var nextNextMonth = now.AddMonths(2);
                    var nextMonthFirstDay = new DateTimeOffset(nextMonth.Year, nextMonth.Month, 1, 0, 0, 0, timeSpan);
                    var nextNextMonthFirstDay =
                        new DateTimeOffset(nextNextMonth.Year, nextNextMonth.Month, 1, 0, 0, 0, timeSpan);
                    return (now,
                        nextMonthFirstDay.AddDays(-1),
                        nextMonthFirstDay,
                        nextNextMonthFirstDay.AddDays(-1));
                default:
                    return (null, null, null, null);
            }
        }
    }
}