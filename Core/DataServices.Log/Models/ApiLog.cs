using System;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log.Events;

namespace TGS.Core.DataServices.Log.Models
{
    public class ApiLog
    {
        public Guid Id { get; set; }
        public Guid TrackingId { get; set; }
        public string Ip { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string Prefix { get; set; }
        public string ServerName { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestContent { get; set; }
        public string ResponseHeaders { get; set; }
        public string ResponseContent { get; set; }
        public int ResponseCode { get; set; }
        public int ExecutionTimeMs { get; set; }
        public DateTimeOffset RecordedOn { get; set; }

        public static explicit operator ApiLog(ApiLogEvent a)
        {
            return new ApiLog
            {
                Id = GuidExtensions.NewSequentialGuid(),
                TrackingId = a.trackingid,
                Ip = a.ip,
                Path = a.path,
                Method = a.method,
                Prefix = a.prefix,
                ServerName = a.servername,
                RequestHeaders = a.requestheaders,
                RequestContent = a.requestcontent,
                ResponseHeaders = a.responseheaders,
                ResponseContent = a.responsecontent,
                ResponseCode = a.responsecode,
                ExecutionTimeMs = a.executiontimems,
                RecordedOn = a.recordedon
            };
        }
    }
}
