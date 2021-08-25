using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.WGSports.Outgoing
{
    public record WGSportsAccessMoneyQueryRequest
    {
        public string OrderId { get; init; }
    }
}
