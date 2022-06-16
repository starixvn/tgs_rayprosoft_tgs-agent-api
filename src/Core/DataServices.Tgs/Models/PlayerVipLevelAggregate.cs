using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Tgs.Models;

public class PlayerVipLevelAggregate
{
    public Guid Id { get; set; }
    public Guid BrandId { get; set; }
    public string PlayerCode { get; set; }
    public DateTimeOffset FromOn { get; set; }
    public DateTimeOffset ToOn { get; set; }
    public decimal DepositAmount { get; set; }
    public decimal Turnover { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset UpdateOn { get; set; }
}
