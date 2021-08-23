using System;

namespace TGS.BackOffice.Repositories
{
    public interface ISystemClock
    {
        DateTimeOffset UtcNow { get; }
    }
}
