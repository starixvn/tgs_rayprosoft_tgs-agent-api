using System;
using System.Threading.Tasks;

namespace TGS.Services.Agent;

public interface IMaintenanceRedisRepository
{
    ValueTask<string?> GetBrandMaintenanceAsync(Guid id);

    ValueTask<string?> GetLicenseeMaintenanceAsync(Guid id);

    ValueTask<string?> GetResellerMaintenanceAsync(Guid ids);
}
