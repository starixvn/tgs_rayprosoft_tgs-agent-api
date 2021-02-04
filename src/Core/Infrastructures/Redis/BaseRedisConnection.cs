using System;
using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis
{
    public abstract class BaseRedisConnection : IRedisConnection
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        protected BaseRedisConnection(string connectionString)
        {
            _lazyConnection = new Lazy<ConnectionMultiplexer>(
                () =>
                {
                    var connection = ConnectionMultiplexer.Connect(connectionString);
                    Console.WriteLine($"{nameof(ConnectionMultiplexer)} connection opened.");

                    connection.ConnectionFailed += OnConnectionFailed;
                    connection.ConnectionRestored += ConnectionRestored;
                    connection.ErrorMessage += ErrorMessage;
                    connection.InternalError += InternalError;

                    return connection;
                });
        }

        IConnectionMultiplexer IRedisConnection.GetConnection()
        {
            return _lazyConnection.Value;
        }

        void IDisposable.Dispose()
        {
            if (_lazyConnection.IsValueCreated)
            {
                _lazyConnection.Value.Dispose();
                Console.WriteLine($"{nameof(ConnectionMultiplexer)} connection closed.");
            }
        }

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine($@"{nameof(ConnectionMultiplexer)} connection failed.
FailureType is {e.FailureType}.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }

        private void ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine($@"{nameof(ConnectionMultiplexer)} connection restored.
FailureType is {e.FailureType}.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }

        private void ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine($@"{nameof(ConnectionMultiplexer)} got an error message: {e.Message}.
Endpoint is {e.EndPoint}.");
        }

        private void InternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine($@"{nameof(ConnectionMultiplexer)} internal error.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Origin is {e.Origin}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }
    }
}