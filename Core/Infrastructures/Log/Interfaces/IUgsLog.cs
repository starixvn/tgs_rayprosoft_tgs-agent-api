using System;
using System.Runtime.CompilerServices;
using TGS.Infrastructures.Log.Events;

namespace TGS.Infrastructures.Log.Interfaces
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

        void LogTrackingData(string trackingData);

        void LogApi(ApiCallEvent apiCall);
        void LogAdmin(AdminLogEvent adminEvent);
    }
}
