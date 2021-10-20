using System;
using Tgs.Logging;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class LogRecord
    {
        public string Exception { get; set; }

        public Guid Id { get; set; }

        public TgsLogLevel Level { get; set; }

        public string Logger { get; set; }

        public string Message { get; set; }

        public DateTimeOffset RecordedOn { get; set; }

        public string ServerName { get; set; }

        public Guid TrackingId { get; set; }

        public static explicit operator LogRecord(LogRecordEvent a) =>
            new()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                TrackingId = a.TrackingId,
                ServerName = a.ServerName,
                RecordedOn = a.RecordedOn,
                Exception = a.Exception ?? string.Empty,
                Message = a.Message,
                Level = a.Level,
                Logger = a.Logger
            };
    }
}
