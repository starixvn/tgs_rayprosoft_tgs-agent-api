using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Infrastructures.Utility
{
    public static class BrandSettlementIntervalGenerator
    {
        public static (DateTimeOffset? BeforeFromDate, DateTimeOffset? BeforeToDate, DateTimeOffset? AfterFromDate,
            DateTimeOffset? AfterToDate) GetInterval(
                SettlementIntervalType? type)
        {
            var now = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, DateTimeOffset.Now.Day, 0,
                0, 0, new(8, 0, 0));

            switch (type)
            {
                case SettlementIntervalType.Daily:
                    return (now, now, now.AddDays(1), now.AddDays(1));
                case SettlementIntervalType.Weekly:
                    return (now.Subtract(new TimeSpan((int)now.DayOfWeek - 1, 0, 0, 0)),
                        now.Subtract(new TimeSpan((int)now.DayOfWeek - 7, 0, 0, 0)),
                        now.Subtract(new TimeSpan((int)now.DayOfWeek - 8, 0, 0, 0)),
                        now.Subtract(new TimeSpan((int)now.DayOfWeek - 14, 0, 0, 0)));
                case SettlementIntervalType.Monthly:
                    var nextMonth = DateTimeOffset.Now.AddMonths(1);
                    var nextNextMonth = DateTimeOffset.Now.AddMonths(2);
                    return (new(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1, 0, 0, 0, new(8, 0, 0)),
                        new(nextMonth.AddDays(-1).Year, nextMonth.AddDays(-1).Month, nextMonth.AddDays(-1).Day, 0, 0, 0,
                            new(8, 0, 0)),
                        new(nextMonth.Year, nextMonth.Month, 1, 0, 0, 0, new(8, 0, 0)),
                        new(nextNextMonth.AddDays(-1).Year, nextNextMonth.AddDays(-1).Month,
                            nextNextMonth.AddDays(-1).Day, 0, 0, 0, new(8, 0, 0)));
                default:
                    return (null, null, null, null);
            }
        }
    }
}