using System;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log.Events;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class AdminLog
    {
        public Guid TrackingId { get; set; }
        public Guid Id { get; set; }
        public string Ip { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
        public string ServerName { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestContent { get; set; }
        public string ResponseHeaders { get; set; }
        public string ResponseContent { get; set; }
        public int ResponseCode { get; set; }
        public int ExecutionTimeMs { get; set; }
        public DateTimeOffset RecordedOn { get; set; }

        public static explicit operator AdminLog(AdminLogEvent a) =>
            new()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                TrackingId = a.trackingid,
                Ip = a.ipaddress ?? string.Empty,
                Path = a.path ?? string.Empty,
                Method = a.method ?? string.Empty,
                Description = a.description ?? string.Empty,
                Exception = a.exception ?? string.Empty,
                ServerName = a.servername ?? string.Empty,
                RequestHeaders = a.requestheaders ?? string.Empty,
                RequestContent = a.requestcontent ?? string.Empty,
                ResponseHeaders = a.responseheaders ?? string.Empty,
                ResponseContent = a.responsecontent ?? string.Empty,
                ResponseCode = a.responsecode,
                ExecutionTimeMs = a.executiontimems,
                RecordedOn = a.recordedon,
                StackTrace = a.stackTrace ?? string.Empty,
                InnerException = a.innerException ?? string.Empty
            };
    }
}