using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Paging;
using TGS.Services.Operator.ApplicationServices.Vip;

namespace TGS.Services.Operator
{
    public interface IVipLevelRepository
    {
        IAsyncEnumerable<VipRecord> QueryVipLevelsAsync(Guid brandId, Paging<VipRecord> paging);

        IAsyncEnumerable<VipRecord> QueryVipLevelsAsync(Guid brandId);

        ValueTask<int> QueryVipLevelsTotalCountAsync(Guid brandId);
    }
}
