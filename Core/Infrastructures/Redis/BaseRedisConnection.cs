using System;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Log;

namespace TGS.Core.Infrastructures.Redis
{
    public abstract class BaseRedisConnection : IRedisConnection
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        private readonly IServiceLog<BaseRedisConnection> _serviceLog;

        protected BaseRedisConnection(string connectionString, IServiceLog<BaseRedisConnection> serviceLog)
        {
            _serviceLog = serviceLog;

            _lazyConnection = new Lazy<ConnectionMultiplexer>(
                () =>
                {
                    var connection = ConnectionMultiplexer.Connect(connectionString);
                    _serviceLog.LogInformation($"{nameof(ConnectionMultiplexer)} connection opened.");

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
                _serviceLog.LogInformation($"{nameof(ConnectionMultiplexer)} connection closed.");
            }
        }

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _serviceLog.LogWarning($@"{nameof(ConnectionMultiplexer)} connection failed.
FailureType is {e.FailureType}.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }

        private void ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _serviceLog.LogInformation($@"{nameof(ConnectionMultiplexer)} connection restored.
FailureType is {e.FailureType}.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }

        private void ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            _serviceLog.LogError($@"{nameof(ConnectionMultiplexer)} got an error message: {e.Message}.
Endpoint is {e.EndPoint}.");
        }

        private void InternalError(object sender, InternalErrorEventArgs e)
        {
            _serviceLog.LogError($@"{nameof(ConnectionMultiplexer)} internal error.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Origin is {e.Origin}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }
    }
}