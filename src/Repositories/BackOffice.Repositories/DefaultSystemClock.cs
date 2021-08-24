using System;

namespace TGS.BackOffice.Repositories
{
    internal class DefaultSystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
