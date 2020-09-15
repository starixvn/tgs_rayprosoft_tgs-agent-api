using System;
using TGS.Infrastructures.Extensions;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.DataService.Models
{
    public class AdminLog
    {
        public Guid Id { get; set; }
        public string Ip { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public string ServerName { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestContent { get; set; }
        public string ResponseHeaders { get; set; }
        public string ResponseContent { get; set; }
        public int ResponseCode { get; set; }
        public int ExecutionTimeMs { get; set; }
        public DateTimeOffset RecordedOn { get; set; }

        public static explicit operator AdminLog(AdminLogEvent a)
        {
            return new AdminLog
            {
                Id = GuidExtensions.NewSequentialGuid(),
                Ip = a.ipaddress,
                Path = a.path,
                Method = a.method,
                Description = a.description,
                Exception = a.exception,
                ServerName = a.servername,
                RequestHeaders = a.requestheaders,
                RequestContent = a.requestcontent,
                ResponseHeaders = a.responseheaders,
                ResponseContent = a.responsecontent,
                ResponseCode = a.responsecode,
                ExecutionTimeMs = a.executiontimems,
                RecordedOn = a.recordedon,
                StackTrace = a.StackTrace
            };
        }
    }
}
