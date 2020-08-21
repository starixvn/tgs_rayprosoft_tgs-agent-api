using System;
using TGS.Infrastructures.Extensions;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.DataService.Models
{
    public class EventLog
    {
        public Guid Id { get; set; }
        public string ServerName { get; set; }
        public string ServiceName { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public DateTimeOffset RecordedOn { get; set; }


        public static explicit operator EventLog(EventLogEvent a)
        {
            return new EventLog
            {
                Id = GuidExtensions.NewSequentialGuid(),
                ServerName = a.servername,
                ServiceName = a.servicename,
                Message = a.message,
                Exception = a.exception,
                RecordedOn = a.recordedon
            };
        }
    }
}
