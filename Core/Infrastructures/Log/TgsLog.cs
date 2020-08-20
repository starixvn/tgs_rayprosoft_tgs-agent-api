using System;
using TGS.Infrastructures.Log.Enums;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Log.Interfaces;

namespace TGS.Infrastructures.Log
{
    internal sealed class TgsLog : IUgsLog
    {
        internal const string LogPublisher = "LogPublisher";
        private string _loggerName;
        private readonly Action<LogBusEvent> _logPublisher;

        //public UgsLog(
        //    [Dependency(LogPublisher)] Action<LogBusEvent> logPublisher)
        //{
        //    _logPublisher = logPublisher;
        //}

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

        IUgsLog IUgsLog.SetLogger(string name)
        {
            _loggerName = name;
            return this;
        }

        void IUgsLog.LogInfo(string message, Exception exception, string file, string member, int line)
            => SendLogRecord(UgsLogLevel.Info, message, exception, file, member, line);

        void IUgsLog.LogDebug(string message, Exception exception, string file, string member, int line)
            => SendLogRecord(UgsLogLevel.Debug, message, exception, file, member, line);

        void IUgsLog.LogWarning(string message, Exception exception, string file, string member, int line)
            => SendLogRecord(UgsLogLevel.Warning, message, exception, file, member, line);

        void IUgsLog.LogError(string message, Exception exception, string file, string member, int line)
            => SendLogRecord(UgsLogLevel.Error, message, exception, file, member, line);

        void IUgsLog.LogFatal(string message, Exception exception, string file, string member, int line)
            => SendLogRecord(UgsLogLevel.Fatal, message, exception, file, member, line);

        void IUgsLog.LogTrackingData(string trackingData)
            => _logPublisher(new LogRecordEvent
            {
                trackingid = TrackingId,
                recordedon = DateTimeOffset.UtcNow,
                servername = Environment.MachineName,
                level = UgsLogLevel.Info,
                logger = trackingData
            });

        void IUgsLog.LogApi(ApiCallEvent apiCallEvent)
        {
            apiCallEvent.trackingid = TrackingId;
            _logPublisher(apiCallEvent);
        }

        void IUgsLog.LogAdmin(AdminLogEvent adminEvent)
        {
            adminEvent.trackingid = TrackingId;
            _logPublisher(adminEvent);
        }

        private void SendLogRecord(UgsLogLevel level, string message, Exception exception,
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