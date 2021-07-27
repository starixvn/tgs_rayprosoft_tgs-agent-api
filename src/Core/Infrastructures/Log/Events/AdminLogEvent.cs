using System;
using MessagePack;

// ReSharper disable InconsistentNaming

namespace TGS.Core.Infrastructures.Log.Events
{
    [MessagePackObject(true)]
    public class AdminLogEvent : LogBusEvent
    {
        public Guid trackingid { get; set; }
        public string path { get; set; }
        public string method { get; set; }
        public string host { get; set; }
        public string servername { get; set; }
        public string requestheaders { get; set; }
        public string requestcontent { get; set; }
        public string responseheaders { get; set; }
        public string responsecontent { get; set; }
        public string StackTrace { get; set; }
        public string exception { get; set; }
        public string InnerException { get; set; }
        public string description { get; set; }
        public string ipaddress { get; set; }
        public int responsecode { get; set; }
        public int executiontimems { get; set; }
        public DateTimeOffset recordedon { get; set; }
    }
}