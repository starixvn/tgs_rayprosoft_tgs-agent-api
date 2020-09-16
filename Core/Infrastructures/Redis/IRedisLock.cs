using System;
using System.Threading.Tasks;

namespace TGS.Core.Infrastructures.Redis
{
    public interface IRedisLock
    {
        Task<TResult> ExecuteLockAsync<TResult>(string resource, TimeSpan expiryTime, Func<IRedisLockSummary, Task<TResult>> execFunc, Func<IRedisLockSummary, TResult> handleFailureFunc, TimeSpan? waitTime = null, TimeSpan? retryInterval = null);

        Task ExecuteLockAsync(string resource, TimeSpan expiryTime, Func<Task> execFunc, Func<Task> handleFailureFunc = null, TimeSpan? waitTime = null, TimeSpan? retryInterval = null);
    }
}