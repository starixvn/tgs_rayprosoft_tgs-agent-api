using System;
using TGS.Infrastructures.Log.Enums;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Log.Interfaces;

namespace TGS.Infrastructures.Log
{
    internal sealed class TgsLog : ITgsLog
    {
        internal const string LogPublisher = "LogPublisher";
        private string _loggerName;
        private readonly Action<LogBusEvent> _logPublisher;

        public TgsLog(Action<LogBusEvent> logPublisher)
        {
            _logPublisher = logPublisher;
        }

        private const string TrackingIdCallContextSlotName = "callcontext::trackingid";

        public Guid TrackingId
        {
            get => CallContext<Guid>.GetData(TrackingIdCallContextSlotName);
            set
            {
                var slot = CallContext<Guid>.GetData(TrackingIdCallContextSlotName);
                if (slot == Guid.Empty)
                {
                    CallContext<Guid>.SetData(TrackingIdCallContextSlotName, value);
                }
            }
        }

        Guid ITgsLog.TrackingId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ITgsLog ITgsLog.SetLogger(string name)
        {
            _loggerName = name;
            return this;
        }

        void ITgsLog.LogInfo(string message, Exception exception, string file, string member, int line)
            => SendLogRecord(TgsLogLevel.Info, message, exception, file, member, line);

        void ITgsLog.LogDebug(string message, Exception exception, string file, string member, int line)
            => SendLogRecord(TgsLogLevel.Debug, message, exception, file, member, line);

        void ITgsLog.LogWarning(string message, Exception exception, string file, string member, int line)
            => SendLogRecord(TgsLogLevel.Warning, message, exception, file, member, line);

        void ITgsLog.LogError(string message, Exception exception, string file, string member, int line)
            => SendLogRecord(TgsLogLevel.Error, message, exception, file, member, line);

        void ITgsLog.LogFatal(string message, Exception exception, string file, string member, int line)
            => SendLogRecord(TgsLogLevel.Fatal, message, exception, file, member, line);

        void ITgsLog.LogTrackingData(string trackingData)
            => _logPublisher(new LogRecordEvent
            {
                trackingid = TrackingId,
                recordedon = DateTimeOffset.UtcNow,
                servername = Environment.MachineName,
                level = TgsLogLevel.Info,
                logger = trackingData
            });

        void ITgsLog.LogApi(ApiLogEvent apiLogEvent)
        {
            apiLogEvent.trackingid = TrackingId;
            _logPublisher(apiLogEvent);
        }

        void ITgsLog.LogOs(OsLogEvent logEvent)
        {
            logEvent.trackingid = TrackingId;
            logEvent.servicename = logEvent.servicename;
            logEvent.message = logEvent.message;
            logEvent.exception = logEvent.exception;
            logEvent.recordedon = DateTimeOffset.UtcNow;
            _logPublisher(logEvent);
        }

        private void SendLogRecord(TgsLogLevel level, string message, Exception exception,
            string callerFile, string callerMember, int lineNumber)
        {
            _logPublisher(new LogRecordEvent
            {
                trackingid = TrackingId,
                recordedon = DateTimeOffset.UtcNow,
                servername = Environment.MachineName,
                level = level,
                logger = _loggerName,
                message = message,
                exception = exception?.ToString(),
                file = callerFile,
                member = callerMember,
                line = lineNumber
            });
        }
    }
}