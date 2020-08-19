using System;

namespace TGS.Infrastructures.Redis
{
    public interface IRedisLockSummary : IDisposable
    {
        bool IsAcquired { get; }
        bool IsReleased { get; }
    }
}