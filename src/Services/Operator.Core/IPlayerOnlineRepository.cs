using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.Operator;

public interface IPlayerOnlineRepository
{
    Task SetAsync(Guid brandId, string playerCode);
}
