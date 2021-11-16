using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BetConstruct.Incoming
{
    public record RollbackRequest : BaseRequest
    {
        public long TransactionId { get; set; }
        public long? BetId { get; set;}
    }
}
