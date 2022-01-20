using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerVipLevelSeasonPromotion
    {
        public Guid Id { get; set; }
        public Guid VipId { get; set; }
        public Guid PlayerVipLevelSnapshotId { get; set; }
        public string PlayerCode { get; set; }
        public DateTimeOffset SeasonOn { get; set; }
        public DateTimeOffset FromOn { get; set; }
        public DateTimeOffset ToOn { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
