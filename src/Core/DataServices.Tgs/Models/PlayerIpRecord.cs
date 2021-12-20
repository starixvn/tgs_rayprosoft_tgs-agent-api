using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerIpRecord
    {
        public string Host { get; set; }

        public string Ip { get; set; }

        public int IpType { get; set; }

        public string PlayerCode { get; set; }

        public DateTimeOffset RecordOn { get; set; }
    }
}
