using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Tgs.Models;

public class PlayerVipLevelUpgradePromotion
{
    public Guid Id { get; set; }
    public Guid VipId { get; set; }
    public Guid PlayerVipLevelSnapshotId { get; set; }
    public string PlayerCode { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
}
