using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Tgs.Models;

public class RoundOperationRecord
{
    public Guid Id { get; set; }
    public Guid RoundId { get; set; }
    public Guid OperatorId { get; set; }
    public string OldData { get; set; }
    public string NewData { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string Remark { get; set; }
}
