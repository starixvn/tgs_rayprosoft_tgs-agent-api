using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Models;
using TGS.Services.GameOperations;

namespace TGS.Tests.Games.IntegrationTests;

internal class StubRedisPlayerLoginRecordService : IRedisPlayerLoginRecordService
{
    private readonly IPlayerLoginRecordRepository _playerLoginRecordRepository;

    public StubRedisPlayerLoginRecordService(
        IPlayerLoginRecordRepository playerLoginRecordRepository)
    {
        _playerLoginRecordRepository = playerLoginRecordRepository ?? throw new ArgumentNullException(nameof(playerLoginRecordRepository));
    }

    public Task<PlayerLoginRecordModel> GetAsync(Guid loginRecordId)
        => _playerLoginRecordRepository.GetAsync(loginRecordId);

    public async Task<PlayerLoginRecordModel> GetLastAsync(string playerCode)
    {
        var loginRecords = await QueryEnabledAsync(playerCode).ConfigureAwait(false);
        return loginRecords.FirstOrDefault();
    }

    public async Task<PlayerLoginRecordModel> GetLastAsync(string playerCode, PlatformType platform)
    {
        var loginRecords = await QueryEnabledAsync(playerCode).ConfigureAwait(false);
        return loginRecords.FirstOrDefault(r => r.PlatformType == platform);
    }

    public Task<PlayerLoginRecordModel[]> QueryEnabledAsync(string playerCode)
        => _playerLoginRecordRepository.QueryEnabledAsync(playerCode);
}
