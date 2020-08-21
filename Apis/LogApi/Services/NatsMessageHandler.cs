using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NATS.Client;
using TGS.Apis.LogApi.Interfaces;
using TGS.Core.Models;
using TGS.Infrastructures.Extensions;
using TGS.Infrastructures.Log;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Nats;

namespace TGS.Apis.LogApi.Services
{
    public interface IMessageHandler
    {
        void StartReceiveMessages();

        void StopReceiveMessages();

        DateTimeOffset LastStartUpTime { get; }
        DateTimeOffset LastReceivedEventTime { get; }
        DateTimeOffset LastLogInsertTime { get; }
        string LastLogInsertError { get; }
        LogBusEvent LastBusEvent { get; }
        ConcurrentDictionary<string, uint> LogReceivedCount { get; }
    }

    public class NatsMessageHandler : IMessageHandler
    {
        private readonly Func<string, ILoggerEventWriter> _logWriterFactory;
        private readonly RequestAnalyzer _osLog;
        private readonly INatsManager _natsManager;
        private IAsyncSubscription _ugsAdminLogSubscription;
        private IAsyncSubscription _ugsApiCallLogSubscription;
        private IAsyncSubscription _ugsLogRecordSubscription;
        private DateTimeOffset _lastReceivedEventTime;
        private LogBusEvent _lastBusEvent;
        private DateTimeOffset _startupTime;
        private DateTimeOffset _lastInsertTime;
        private string _lastError;
        private IDisposable _subscription;
        private readonly ConcurrentDictionary<string, uint> _logReceivedCount = new ConcurrentDictionary<string, uint>();
        private readonly IOptions<LoggerServiceConfig> _loggerServiceConfig;
        public NatsMessageHandler(
            Func<string, ILoggerEventWriter> logWriterFactory,
            RequestAnalyzer osLog,
            IOptions<LoggerServiceConfig> loggerServiceConfig,
            INatsManager natsManager)
        {
            _logWriterFactory = logWriterFactory;
            _osLog = osLog;
            _natsManager = natsManager;
            _loggerServiceConfig = loggerServiceConfig;
        }

        void IMessageHandler.StartReceiveMessages()
        {
            SubscribeAdminLogReceived();
            SubscribeApiCallLogReceived();
            SubscribeLogRecordReceived();
            RegisterAdminLogSubscriberClient();
            RegisterApiCallLogSubscriberClient();
            RegisterLogRecordSubscriberClient();
            _startupTime = DateTimeOffset.UtcNow;
        }

        public event Action<LogBusEvent> LogRecordReceived;

        public event Action<LogBusEvent> ApiCallReceived;

        public event Action<LogBusEvent> AdminLogReceived;

        private void SubscribeAdminLogReceived()
        {
            var query =
                from logs in Observable.FromEvent<LogBusEvent>(
                        ev => AdminLogReceived += ev,
                        ev => AdminLogReceived -= ev)
                    .Buffer(_loggerServiceConfig.Value.BatchInsertWaitingSeconds, _loggerServiceConfig.Value.BatchInsertCounts)
                where logs.Count > 0
                select logs;

            _subscription = query
                .SubscribeOn(NewThreadScheduler.Default)
                .Subscribe(
                    async logs =>
                    {
                        var now = DateTimeOffset.UtcNow;
                        CountReceivedLogs(now, nameof(AdminLogEvent), logs.Count);
                        await WriteLogsAsync(nameof(AdminLogEvent), logs);
                    },
                    ex => _osLog.WriteEventLogError("error occurred when RX executed:" + ex));
        }

        private void SubscribeApiCallLogReceived()
        {
            var query =
                from logs in Observable.FromEvent<LogBusEvent>(
                        ev => ApiCallReceived += ev,
                        ev => ApiCallReceived -= ev)
                    .Buffer(_loggerServiceConfig.Value.BatchInsertWaitingSeconds, _loggerServiceConfig.Value.BatchInsertCounts)
                where logs.Count > 0
                select logs;

            _subscription = query
                .SubscribeOn(NewThreadScheduler.Default)
                .Subscribe(
                    async logs =>
                    {
                        var now = DateTimeOffset.UtcNow;
                        CountReceivedLogs(now, nameof(ApiLogEvent), logs.Count);
                        await WriteLogsAsync(nameof(ApiLogEvent), logs);
                    },
                    ex => _osLog.WriteEventLogError("error occurred when RX executed:" + ex));
        }

        private void SubscribeLogRecordReceived()
        {
            var query =
                from logs in Observable.FromEvent<LogBusEvent>(
                        ev => LogRecordReceived += ev,
                        ev => LogRecordReceived -= ev)
                    .Buffer(_loggerServiceConfig.Value.BatchInsertWaitingSeconds, _loggerServiceConfig.Value.BatchInsertCounts)
                where logs.Count > 0
                select logs;

            _subscription = query
                .SubscribeOn(NewThreadScheduler.Default)
                .Subscribe(
                    async logs =>
                    {
                        var now = DateTimeOffset.UtcNow;
                        CountReceivedLogs(now, nameof(LogRecordEvent), logs.Cast<LogRecordEvent>().Count(log => log.level >= _loggerServiceConfig.Value.LogLevel || _loggerServiceConfig.Value.UnrestrictedLoggers.Contains(log.logger)));
                        await WriteLogsAsync(nameof(LogRecordEvent), logs);
                    },
                    ex => _osLog.WriteEventLogError("error occurred when RX executed:" + ex));
        }

        void IMessageHandler.StopReceiveMessages()
        {
            _ugsAdminLogSubscription?.Dispose();
            _ugsApiCallLogSubscription?.Dispose();
            _ugsLogRecordSubscription?.Dispose();
            Task.WaitAll(Task.Delay(_loggerServiceConfig.Value.BatchInsertWaitingSeconds.Add(TimeSpan.FromSeconds(3))));
            _subscription?.Dispose();
        }

        DateTimeOffset IMessageHandler.LastStartUpTime => _startupTime;

        DateTimeOffset IMessageHandler.LastReceivedEventTime => _lastReceivedEventTime;

        DateTimeOffset IMessageHandler.LastLogInsertTime => _lastInsertTime;

        string IMessageHandler.LastLogInsertError => _lastError;

        LogBusEvent IMessageHandler.LastBusEvent => _lastBusEvent;

        ConcurrentDictionary<string, uint> IMessageHandler.LogReceivedCount => _logReceivedCount;

        private void RegisterAdminLogSubscriberClient()
        {
            _ugsAdminLogSubscription = _natsManager.SubscribeAsync<AdminLogEvent>(
                nameof(AdminLogEvent),
                Consts.NatsUgsLogQueueName,
                message =>
                {
                    _lastBusEvent = message.DataObject;
                    _lastReceivedEventTime = DateTimeOffset.UtcNow;
                    AdminLogReceived?.Invoke(message.DataObject);
                });
        }

        private void RegisterApiCallLogSubscriberClient()
        {
            _ugsApiCallLogSubscription = _natsManager.SubscribeAsync<ApiLogEvent>(
                nameof(ApiLogEvent),
                Consts.NatsUgsLogQueueName,
                message =>
                {
                    _lastBusEvent = message.DataObject;
                    _lastReceivedEventTime = DateTimeOffset.UtcNow;
                    ApiCallReceived?.Invoke(message.DataObject);
                });
        }

        private void RegisterLogRecordSubscriberClient()
        {
            _ugsLogRecordSubscription = _natsManager.SubscribeAsync<LogRecordEvent>(
                nameof(LogRecordEvent),
                Consts.NatsUgsLogQueueName,
                message =>
                {
                    _lastBusEvent = message.DataObject;
                    _lastReceivedEventTime = DateTimeOffset.UtcNow;
                    LogRecordReceived?.Invoke(message.DataObject);
                });
        }

        private async Task WriteLogsAsync(string eventType, IList<LogBusEvent> events)
        {
            if (eventType.IsNullOrEmpty() || events == null)
                await Task.FromResult(0);
            try
            {
                using (var logger = _logWriterFactory(eventType))
                {
                    await logger.SaveLogEventsAsync(events);
                    _lastInsertTime = DateTimeOffset.UtcNow;
                }
            }
            catch (Exception ex)
            {
                _osLog.WriteEventLogError("error occurred when batch insert logs:" + ex);
                _lastError = ex.ToString();
            }
        }

        private void CountReceivedLogs(DateTimeOffset now, string eventTypeName, int? count)
        {
            var key = $"{now:yyyy/MM/dd HH:00} {eventTypeName}";
            if (count == null || count == 0)
                return;
            _logReceivedCount.AddOrUpdate(
                key,
                (uint)count,
                (k, oldCount) => oldCount + (uint)count);
        }
    }
}
