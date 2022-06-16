using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.GameProviders.Repositories.Models;

namespace TGS.Services.GameOperations;

public interface IRedisPlayerLoginRecordService
{
    Task<PlayerLoginRecordModel> GetAsync(Guid loginRecordId);

    Task<PlayerLoginRecordModel> GetLastAsync(string playerCode);

    Task<PlayerLoginRecordModel> GetLastAsync(string playerCode, DateTimeOffset current);

    Task<PlayerLoginRecordModel> GetLastAsync(string playerCode, PlatformType platform);

    Task<PlayerLoginRecordModel> GetLastAsync(string playerCode, PlatformType platform, DateTimeOffset current);

    Task<PlayerLoginRecordModel[]> QueryEnabledAsync(string playerCode);
}
