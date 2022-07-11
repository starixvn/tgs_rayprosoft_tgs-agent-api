using System;

namespace TGS.Services.BackOffice;

public interface IDateLimiter
{
    public (DateOnly From, DateOnly To) FutureToToday(DateTimeOffset from, DateTimeOffset to, TimeSpan offset);
}
