using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.PlayerOnline;

namespace TGS.Services.BackOffice.Domain
{
    public interface IPlayerOnlineRepository
    {
        ValueTask<string[]> GetQueryAsync(Guid brandId);
    }
}
