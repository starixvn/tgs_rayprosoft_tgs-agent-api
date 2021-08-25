using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.WGHorse.Outgoing
{
    public record WGHorseAccessMoneyQueryRequest
    {
        public string OrderId { get; init; }
    }
}
