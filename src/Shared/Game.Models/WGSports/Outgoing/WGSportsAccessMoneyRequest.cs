using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.WGSports.Enums;

namespace TGS.Shared.Game.Models.WGSports.Outgoing
{
    public record WGSportsAccessMoneyRequest
    {
        public string PlayerCode { get; init; }
        public AmountTypes AmountType { get; init; }
        public string OrderId { get; init; }
        public string Amount { get; init; }
    }
}
