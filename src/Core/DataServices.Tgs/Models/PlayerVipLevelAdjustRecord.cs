using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Tgs.Models;

public class PlayerVipLevelAdjustRecord
{
    public Guid Id { get; set; }
    public string PlayerCode { get; set; }
    public Guid OldVipId { get; set; }
    public Guid NewVipId { get; set; }
    public Guid OperatorId { get; set; }
    public string Remark { get; set; }
    public DateTimeOffset CreatedOn { get; set; }

}
