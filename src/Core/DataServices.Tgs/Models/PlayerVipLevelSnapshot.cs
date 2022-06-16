using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Tgs.Models;

public class PlayerVipLevelSnapshot
{
    public Guid Id { get; set; }
    public Guid BrandId { get; set; }
    public DateTimeOffset SnapshotOn { get; set; }
    public string Data { get; set; }

    public DateTimeOffset CreatedOn { get; set; }
}
