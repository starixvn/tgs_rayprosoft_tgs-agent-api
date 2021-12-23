using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.GameProviders.Repositories.Models;

namespace TGS.Services.GameOperations
{
    public interface IPlayerLoginRecordService
    {
        Task<PlayerLoginRecordModel> GetAsync(Guid loginRecordId);

        Task<PlayerLoginRecordModel> GetLastAsync(string playerCode);

        Task<PlayerLoginRecordModel> GetLastAsync(string playerCode, PlatformType platform);

        Task<PlayerLoginRecordModel[]> QueryEnabledAsync(string playerCode);
    }
}
