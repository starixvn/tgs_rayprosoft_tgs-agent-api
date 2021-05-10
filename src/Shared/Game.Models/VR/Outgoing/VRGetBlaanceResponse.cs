using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRGetBlaanceResponse
    {
        public string playerName { get; set; }
        public float balance { get; set; }
        public IEnumerable<VRGetBlaanceGamesResponse> games { get; set; }
    }
}
