using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRGetBalanceResponse
    {
        public string playerName { get; set; }
        public decimal balance { get; set; }
        public IEnumerable<VRGetBalanceGamesResponse> games { get; set; }
    }
}
