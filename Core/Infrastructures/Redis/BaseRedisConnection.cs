using System;
using StackExchange.Redis;
using TGS.Infrastructures.Log;

namespace TGS.Infrastructures.Redis
{
    public abstract class BaseRedisConnection : IRedisConnection
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        private readonly RequestAnalyzer _osLog;

        protected BaseRedisConnection(string connectionString, RequestAnalyzer osLog)
        {
            _osLog = osLog;

            _lazyConnection = new Lazy<ConnectionMultiplexer>(
                () =>
                {
                    var connection = ConnectionMultiplexer.Connect(connectionString);
                    _osLog.WriteEventLogInfo($"{nameof(ConnectionMultiplexer)} connection opened.");

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
                _osLog.WriteEventLogInfo($"{nameof(ConnectionMultiplexer)} connection closed.");
            }
        }

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _osLog.WriteEventLogWarning($@"{nameof(ConnectionMultiplexer)} connection failed.
FailureType is {e.FailureType}.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }

        private void ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _osLog.WriteEventLogInfo($@"{nameof(ConnectionMultiplexer)} connection restored.
FailureType is {e.FailureType}.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }

        private void ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            _osLog.WriteEventLogError($@"{nameof(ConnectionMultiplexer)} got an error message: {e.Message}.
Endpoint is {e.EndPoint}.");
        }

        private void InternalError(object sender, InternalErrorEventArgs e)
        {
            _osLog.WriteEventLogError($@"{nameof(ConnectionMultiplexer)} internal error.
ConnectionType is {e.ConnectionType}.
Endpoint is {e.EndPoint}.
Origin is {e.Origin}.
Exception is {e.Exception?.ToString() ?? "null"}");
        }
    }
}