using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Commons
{
    public record VRApiRequest
    {
        public string version { get; set; }
        public string id { get; set; }
        public string data { get; set; }
    }
}
