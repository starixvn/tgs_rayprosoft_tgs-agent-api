using System;
using MessagePack;
using TGS.Infrastructures.Log.Enums;

// ReSharper disable InconsistentNaming

namespace TGS.Infrastructures.Log.Events
{
    [MessagePackObject((true))]
    public class AdminLogEvent : LogBusEvent
    {
        public Guid trackingid { get; set; }
        public string ipaddress { get; set; }
        public string path { get; set; }
        public string method { get; set; }
        public string prefix { get; set; }
        public string servername { get; set; }
        public string requestheaders { get; set; }
        public string requestcontent { get; set; }
        public string responseheaders { get; set; }
        public string responsecontent { get; set; }
        public int responsecode { get; set; }
        public int executiontimems { get; set; }
        public string description { get; set; }
        public DateTimeOffset recordedon { get; set; }
        public AdminLogType adminlogtype { get; set; }
        public Guid? userid { get; set; }
        public Guid? licenseeid { get; set; }
        public Guid? resellerid { get; set; }

        public string username { get; set; }
        public string exception { get; set; }
    }
}
