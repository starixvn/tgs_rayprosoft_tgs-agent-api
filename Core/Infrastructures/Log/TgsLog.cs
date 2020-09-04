using System;
using System.Runtime.CompilerServices;
using TGS.Infrastructures.Log.Enums;
using TGS.Infrastructures.Log.Events;

namespace TGS.Infrastructures.Log
{
    public interface ITgsLog
    {
        Guid TrackingId { get; set; }

        ITgsLog SetLogger(string name);

        void LogInfo(string message, Exception exception = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0);

        void LogDebug(string message, Exception exception = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0);

        void LogWarning(string message, Exception exception = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0);

        void LogError(string message, Exception exception = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0);

        void LogFatal(string message, Exception exception = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0);

        void WriteEventLogError(string errorMessage, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        void WriteEventLogWarning(string warningMessage, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        void WriteEventLogInfo(string infoMessage, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        void LogTrackingData(string trackingData);

        void LogApi(ApiLogEvent apiCall);

        void LogAdmin(AdminLogEvent adminEvent);
    }

    public sealed class TgsLog : ITgsLog
    {
        public const string LogPublisher = "LogPublisher";
        private string _loggerName;
        private readonly Action<LogBusEvent> _logPublisher;

        public TgsLog(Action<LogBusEvent> logPublisher)
        {
            _logPublisher = logPublisher;
        }

        private const string TrackingIdCallContextSlotName = "callcontext::trackingid";

        public Guid TrackingId
        {
            get => Services.CallContext<Guid>.GetData(TrackingIdCallContextSlotName);
            set
            {
                var slot = Services.CallContext<Guid>.GetData(TrackingIdCallContextSlotName);
                if (slot == Guid.Empty)
                {
                    Services.CallContext<Guid>.SetData(TrackingIdCallContextSlotName, value);
                }
            }
        }

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

        void ITgsLog.WriteEventLogError(string errorMessage, string memberName, string sourceFilePath, int sourceLineNumber)
        {

        }

        void ITgsLog.WriteEventLogWarning(string warningMessage, string memberName, string sourceFilePath, int sourceLineNumber)
        {

        }

        void ITgsLog.WriteEventLogInfo(string infoMessage, string memberName, string sourceFilePath, int sourceLineNumber)
        {

        }


        void ITgsLog.LogAdmin(AdminLogEvent adminEvent)
        {
            _logPublisher(adminEvent);
        }
    }
}