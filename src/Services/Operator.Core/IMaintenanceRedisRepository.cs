using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IMaintenanceRedisRepository
    {
        ValueTask<string> GetResellerMaintenanceAsync(Guid ids);

        ValueTask<string> GetLicenseeMaintenanceAsync(Guid id);

        ValueTask<string> GetBrandMaintenanceAsync(Guid id);
    }
}
