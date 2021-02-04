using System;
using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis
{
    public interface IRedisConnection : IDisposable
    {
        IConnectionMultiplexer GetConnection();
    }
}