using System;

namespace TGS.Services.BackOffice;

internal class DateLimiter : IDateLimiter
{
    private readonly ISystemClock _systemClock;

    public DateLimiter(ISystemClock systemClock)
    {
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public (DateOnly From, DateOnly To) FutureToToday(DateTimeOffset from, DateTimeOffset to, TimeSpan offset)
    {
        var offsetFrom = from.ToOffset(offset);
        var offsetTo = to.ToOffset(offset);
        var now = _systemClock.UtcNow.ToOffset(offset);

        var start = new DateOnly(offsetFrom.Year, offsetFrom.Month, offsetFrom.Day);
        var end = new DateOnly(offsetTo.Year, offsetTo.Month, offsetTo.Day);
        var today = new DateOnly(now.Year, now.Month, now.Day);

        if (end >= today)
            end = today.AddDays(1);

        if (start > today)
            start = today;

        return (start, end);
    }
}
