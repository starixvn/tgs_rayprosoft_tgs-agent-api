using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TGS.Core.DataServices.Log.Models
{
    public class ServiceLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTimeOffset RecordedOn { get; set; }
        public string MachineName { get; set; }
        public string Level { get; set; }
        public string ServerAddress { get; set; }
        public string RemoteAddress { get; set; }
        public string Logger { get; set; }
        public string Port { get; set; }
        public string Properties { get; set; }
        public string Callsite { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string ServerName { get; set; }
        public string Url { get; set; }
        public string Https { get; set; }
        public string StackTrace { get; set; }

    }
}
