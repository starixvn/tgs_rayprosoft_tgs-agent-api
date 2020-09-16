using System;

namespace TGS.Core.Infrastructures.Redis
{
    public interface IRedisLockSummary : IDisposable
    {
        bool IsAcquired { get; }
        bool IsReleased { get; }
    }
}