using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerIpRecord
    {
        public string PlayerCode { get; set; }
        public int IpType { get; set; }
        public string Ip { get; set; }
        public DateTimeOffset RecordOn { get; set; }
    }
}
