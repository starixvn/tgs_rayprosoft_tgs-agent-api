using System;
using StackExchange.Redis;

namespace TGS.Infrastructures.Redis
{
    public interface IRedisConnection : IDisposable
    {
        IConnectionMultiplexer GetConnection();
    }
}