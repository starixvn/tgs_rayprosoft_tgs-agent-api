using System;
using TGS.Infrastructures.Extensions;
using TGS.Infrastructures.Log.Enums;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.DataService.Models
{
    public class LogRecord
    {
        public Guid Id { get; set; }
        public Guid TrackingId { get; set; }
        public DateTimeOffset RecordedOn { get; set; }
        public string ServerName { get; set; }
        public TgsLogLevel Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }

        public static explicit operator LogRecord(LogRecordEvent a)
        {
            return new LogRecord
            {
                Id = GuidExtensions.NewSequentialGuid(),
                TrackingId = a.trackingid,
                ServerName = a.servername,
                RecordedOn = a.recordedon,
                Exception = a.exception,
                Message = a.message,
                Level = a.level,
                Logger = a.logger
            };
        }
    }
}
