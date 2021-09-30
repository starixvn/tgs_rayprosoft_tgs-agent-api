using System;
using Tgs.Logging;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class AdminLog
    {
        public string Description { get; set; }

        public string Exception { get; set; }

        public int ExecutionTimeMs { get; set; }

        public Guid Id { get; set; }

        public string InnerException { get; set; }

        public string Ip { get; set; }

        public string Method { get; set; }

        public string Path { get; set; }

        public DateTimeOffset RecordedOn { get; set; }

        public string RequestContent { get; set; }

        public string RequestHeaders { get; set; }

        public int ResponseCode { get; set; }

        public string ResponseContent { get; set; }

        public string ResponseHeaders { get; set; }

        public string ServerName { get; set; }

        public string StackTrace { get; set; }

        public Guid TrackingId { get; set; }

        public static explicit operator AdminLog(AdminLogEvent a) =>
            new()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                TrackingId = a.TrackingId,
                Ip = a.IpAddress ?? string.Empty,
                Path = a.Path ?? string.Empty,
                Method = a.Method ?? string.Empty,
                Description = a.Description ?? string.Empty,
                Exception = a.Exception ?? string.Empty,
                ServerName = a.ServerName ?? string.Empty,
                RequestHeaders = a.RequestHeaders ?? string.Empty,
                RequestContent = a.RequestContent ?? string.Empty,
                ResponseHeaders = a.ResponseHeaders ?? string.Empty,
                ResponseContent = a.ResponseContent ?? string.Empty,
                ResponseCode = a.ResponseCode,
                ExecutionTimeMs = a.ExecutionTimeMs,
                RecordedOn = a.RecordedOn,
                StackTrace = a.StackTrace ?? string.Empty,
                InnerException = a.InnerException ?? string.Empty
            };
    }
}
