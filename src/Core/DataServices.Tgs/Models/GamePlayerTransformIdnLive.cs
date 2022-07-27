using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.DataServices.Tgs.Models;

public class GamePlayerTransformIdnLive
{
    public int Id { get; set; }
    public string PlayerCode { get; set; } = default!;
    public string Configuration { get; set; } = default!;
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
}
