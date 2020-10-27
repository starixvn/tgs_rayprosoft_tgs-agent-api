using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Log
{
    public class ApiLogModel
    {
        public Guid Id { get; set; }
        public Guid TrackingId { get; set; }
        public string Ip { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string Prefix { get; set; }
        public string ServerName { get; set; }
        public int ResponseCode { get; set; }
        public int ExecutionTimeMs { get; set; }
        public DateTimeOffset RecordedOn { get; set; }
    }
}