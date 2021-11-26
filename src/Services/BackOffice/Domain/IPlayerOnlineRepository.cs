using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Domain
{
    public interface IPlayerOnlineRepository
    {
        ValueTask<string[]> GetQueryAsync(Guid BrandId);
    }
}
